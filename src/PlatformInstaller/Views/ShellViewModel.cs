using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Anotar.Serilog;
using Autofac;
using Caliburn.Micro;
using Janitor;
using Mindscape.Raygun4Net;
using Parameter = Autofac.Core.Parameter;

[SkipWeaving]
public class ShellViewModel : Conductor<object>,
    IDisposable,
    IHandle<AgreedToLicenseEvent>,
    IHandle<RunInstallEvent>,
    IHandle<InstallSucceededEvent>,
    IHandle<InstallFailedEvent>,
    IHandle<RebootRequiredEvent>,
    IHandle<ExitApplicationCommand>,
    IHandle<UninstallProductCommand>,
    IHandle<NavigateHomeCommand>,
    IHandle<StartDotNet452InstallCommand>,
    IHandle<DotNetDownloadCompleteEvent>,
    IHandle<DotNetInstallFailedEvent>,
    IHandle<DotNetInstallCompleteEvent>
{
    LicenseAgreement licenseAgreement;
    ILifetimeScope lifetimeScope;
    IEventAggregator eventAggregator;
    List<string> itemsToInstall;
    ILifetimeScope currentLifetimeScope;
    RaygunClient raygunClient;
    Installer installer;
    CredentialStore credentialStore;
    RuntimeUpgradeManager runtimeUpgradeManager;

    bool installWasAttempted;

    public ShellViewModel(IEventAggregator eventAggregator, LicenseAgreement licenseAgreement, ILifetimeScope lifetimeScope, RaygunClient raygunClient, Installer installer, CredentialStore credentialStore, RuntimeUpgradeManager runtimeUpgradeManager)
    {
        DisplayName = "Platform Installer";
        this.runtimeUpgradeManager = runtimeUpgradeManager;
        this.raygunClient = raygunClient;
        this.installer = installer;
        this.licenseAgreement = licenseAgreement;
        this.lifetimeScope = lifetimeScope;
        this.eventAggregator = eventAggregator;
        this.credentialStore = credentialStore;
        RunStartupSequence();
    }

    public void ActivateModel<T>(params Parameter[] parameters) where T : Screen
    {
        var activeScreen = ActiveItem;
        if (activeScreen != null && activeScreen.IsHandler())
        {
            eventAggregator.Unsubscribe(activeScreen);
        }
        currentLifetimeScope?.Dispose();
        currentLifetimeScope = lifetimeScope.BeginLifetimeScope();
        var model = currentLifetimeScope.Resolve<T>(parameters);
        if (model.IsHandler())
        {
            eventAggregator.Subscribe(model);
        }
        ActivateItem(model);
    }

    public void Exit()
    {
        eventAggregator.Publish<RequestExitApplicationEvent>();
    }

    public void Handle(AgreedToLicenseEvent message)
    {
        licenseAgreement.Agree();
        RunStartupSequence();
    }

    void RunStartupSequence()
    {

        if (!licenseAgreement.HasAgreedToLicense())
        {
            ActivateModel<LicenseAgreementViewModel>();
            return;
        }

        credentialStore.RetrieveSavedCredentials();
        if (!ProxyTester.ProxyTest(credentialStore.Credentials))
        {
            if (credentialStore.Credentials == null)
            {
                LogTo.Warning("Failed to connect to the internet using anonymous credentials");
            }
            else
            {
                LogTo.Warning("Failed to connect to the internet using stored credentials");
            }

            if (ProxyTester.ProxyTest(CredentialCache.DefaultCredentials))
            {
                credentialStore.Credentials = CredentialCache.DefaultCredentials;
                LogTo.Information("Successfully connect to the internet using default credentials");
            }
            else if (ProxyTester.ProxyTest(CredentialCache.DefaultNetworkCredentials))
            {
                credentialStore.Credentials = CredentialCache.DefaultNetworkCredentials;
                LogTo.Information("Successfully connect to the internet using default network credentials");
            }
            else
            {
                LogTo.Information("Prompting for network credentials");
                var proxySettings = new ProxySettingsView(credentialStore)
                {
                    Owner = ShellView.CurrentInstance
                };
                proxySettings.ShowDialog();
                if (proxySettings.Cancelled)
                {
                    Environment.Exit(0);
                }
            }
        }

        if (!runtimeUpgradeManager.Is452orLaterInstalled())
        {
            ActivateModel<DotNetPreReqViewModel>();
            return;
        }
        ActivateModel<SelectItemsViewModel>();
    }

    public async void Handle(RunInstallEvent message)
    {
        installWasAttempted = true;
        itemsToInstall = message.SelectedItems;
        await ActivateInstallingViewModel().ConfigureAwait(false);
    }

    Task ActivateInstallingViewModel()
    {
        ActivateModel<InstallingViewModel>();
        return installer.Install(itemsToInstall);
    }

    public void Handle(InstallSucceededEvent message)
    {
        ActivateModel<SuccessViewModel>(new NamedParameter("installedItemNames", itemsToInstall));
    }

    public void Handle(RebootRequiredEvent message)
    {
        ActivateModel<RebootNeededViewModel>();
    }

    public void Handle(NavigateHomeCommand message)
    {
        ActivateModel<SelectItemsViewModel>();
    }

    public async void Handle(StartDotNet452InstallCommand message)
    {
        ActivateModel<DotNetDownloadViewModel>();
        await runtimeUpgradeManager.Download452WebInstaller();
    }
    
    public void Handle(ExitApplicationCommand message)
    {
        if (!installWasAttempted)
        {
            eventAggregator.Publish<NoInstallAttemptedEvent>();
        }
        TryClose();
    }

    public void Handle(InstallFailedEvent message)
    {
        var reboot = message.Failures.FirstOrDefault(f => f.Contains("reboot is required"));
        if (reboot != null)
        {
            eventAggregator.Publish<RebootRequiredEvent>();
            return;
        }

        ActivateModel<FailedInstallationViewModel>(new NamedParameter("failureReason", message.Reason), new NamedParameter("failures", message.Failures));
    }

    public void Handle(ReportInstallFailedEvent message)
    {
        var messageInfo = $"{message.Failure} - {message.FailureDetails}";
        raygunClient.Send(new ProductInstallException(messageInfo));
    }

    public void Dispose()
    {
        currentLifetimeScope?.Dispose();
    }

    public void Handle(UninstallProductCommand message)
    {
        var uninstallString = RegistryFind.TryFindUninstallString(message.Product);

        if (string.IsNullOrWhiteSpace(uninstallString))
        {
            MessageBox.Show($"Could not find the uninstall command for {message.Product}", "Uninstall was not attempted", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var exeEnds = uninstallString.IndexOf(".exe", StringComparison.OrdinalIgnoreCase) + 4;

        if (exeEnds < 5)
        {
            MessageBox.Show($"Could not parse the uninstall command for {message.Product}", "Uninstall was not attempted", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var exe = uninstallString.Substring(0, exeEnds);
        var arguments = uninstallString.Remove(0, exeEnds);
        try
        {
            var psi = new ProcessStartInfo(exe, arguments);
            psi.UseShellExecute = true;
            Process.Start(psi).WaitForExit();
        }
        catch
        {
            MessageBox.Show($"Failed to run uninstall command for {message.Product}.  Please use Add/Remove programs in Control Panel", "Uninstall was unsuccessful", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        ActivateModel<SelectItemsViewModel>();
    }

    public void Handle(DotNetDownloadCompleteEvent message)
    {
        ActivateModel<DotNetInstallViewModel>();
        runtimeUpgradeManager.InstallDotNet452();
    }

    public void Handle(DotNetInstallFailedEvent message)
    {
       ActivateModel<RebootNeededViewModel>();
    }

    public void Handle(DotNetInstallCompleteEvent message)
    {
        ActivateModel<SelectItemsViewModel>();
    }
}