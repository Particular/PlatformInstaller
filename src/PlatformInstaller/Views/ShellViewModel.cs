using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
    IHandle<NavigateHomeCommand>
{
    LicenseAgreement licenseAgreement;
    ILifetimeScope lifetimeScope;
    IEventAggregator eventAggregator;
    List<string> itemsToInstall;
    ILifetimeScope currentLifetimeScope;
    RaygunClient raygunClient;
    Installer installer;
    CredentialStore credentialStore;

    bool installWasAttempted;

    public ShellViewModel(IEventAggregator eventAggregator, LicenseAgreement licenseAgreement, ILifetimeScope lifetimeScope, RaygunClient raygunClient, Installer installer, CredentialStore credentialStore)
    {
        DisplayName = "Platform Installer";
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
}