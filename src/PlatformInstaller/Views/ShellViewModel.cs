using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
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
    IHandle<DotNetStartInstallWizardCommand>,
    IHandle<DotNetDownloadCompleteEvent>,
    IHandle<DotNetInstallFailedEvent>,
    IHandle<DotNetInstallCompleteEvent>,
    IHandle<FailureEvent>,
    IHandle<ReportInstallFailedEvent>
{
    LicenseAgreement licenseAgreement;
    ILifetimeScope lifetimeScope;
    IEventAggregator eventAggregator;
    List<string> itemsToInstall;
    ILifetimeScope currentLifetimeScope;
    RaygunClient raygunClient;
    Installer installer;
    RuntimeUpgradeManager runtimeUpgradeManager;
    ProxyTester proxyTester;
    CredentialStore credentialStore;


    bool installWasAttempted;

    public ShellViewModel(IEventAggregator eventAggregator, LicenseAgreement licenseAgreement, ILifetimeScope lifetimeScope, RaygunClient raygunClient, Installer installer, ProxyTester proxyTester, RuntimeUpgradeManager runtimeUpgradeManager, CredentialStore credentialStore)
    {
        DisplayName = "Platform Installer";
        this.runtimeUpgradeManager = runtimeUpgradeManager;
        this.raygunClient = raygunClient;
        this.installer = installer;
        this.licenseAgreement = licenseAgreement;
        this.lifetimeScope = lifetimeScope;
        this.eventAggregator = eventAggregator;
        this.proxyTester = proxyTester;
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

        try
        {
            if (proxyTester.AreCredentialsRequired())
            {
                var proxySettings = new ProxySettingsView(proxyTester, credentialStore)
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
        catch(WebException) 
        {
        }

        if (runtimeUpgradeManager.Is452InstallRequired())
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

    public async void Handle(DotNetStartInstallWizardCommand message)
    {
        ActivateModel<DotNetDownloadViewModel>();
        var dotNetIntall = await runtimeUpgradeManager.Download452WebInstaller().ConfigureAwait(false);
        if (dotNetIntall == null)
        {
            eventAggregator.PublishOnUIThread(new DotNetInstallFailedEvent());
        }
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
            Process.Start(psi)?.WaitForExit();
        }
        catch
        {
            MessageBox.Show($"Failed to run uninstall command for {message.Product}.  Please use Add/Remove programs in Control Panel", "Uninstall was unsuccessful", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        ActivateModel<SelectItemsViewModel>();
    }

    public void Handle(DotNetDownloadCompleteEvent message)
    {
        ShellView.CurrentInstance.HideMe();
        runtimeUpgradeManager.InstallDotNet452();
    }

    public void Handle(DotNetInstallFailedEvent message)
    {
        ShellView.CurrentInstance.ShowMe();
        ActivateModel<DotNetInstallFailedViewModel>();
    }

    public void Handle(DotNetInstallCompleteEvent message)
    {
        ShellView.CurrentInstance.ShowMe();
        ActivateModel<DotNetInstallCompleteViewModel>();
    }

    public void Handle(FailureEvent message)
    {
        ActivateModel<FailureViewModel>( 
            new NamedParameter("FailureDescription", message.FailureDescription), 
            new NamedParameter("FailureText", message.FailureText));
    }
}