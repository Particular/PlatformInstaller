using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
    IHandle<NavigateHomeCommand>,
    IHandle<ReportInstallFailedEvent>
{
    LicenseAgreement licenseAgreement;
    ILifetimeScope lifetimeScope;
    IEventAggregator eventAggregator;
    List<string> itemsToInstall;
    ILifetimeScope currentLifetimeScope;
    RaygunClient raygunClient;
    Installer installer;
    ReleaseManager releaseManager;

    bool installWasAttempted;

    public ShellViewModel(IEventAggregator eventAggregator, LicenseAgreement licenseAgreement, ILifetimeScope lifetimeScope, RaygunClient raygunClient, Installer installer, ReleaseManager releaseManager, IWindowManager windowManager)
    {
        // ReSharper disable once DoNotCallOverridableMethodsInConstructor
        DisplayName = "Platform Installer";
        this.raygunClient = raygunClient;
        this.installer = installer;
        this.licenseAgreement = licenseAgreement;
        this.lifetimeScope = lifetimeScope;
        this.eventAggregator = eventAggregator;
        this.releaseManager = releaseManager;
        RunStartupSequence();
    }

    public void ActivateModel<T>(params Parameter[] parameters) where T : Screen
    {
        var activeScreen = ActiveItem;
        if (activeScreen != null && activeScreen.IsHandler())
        {
            eventAggregator.Unsubscribe(activeScreen);
        }
        if (currentLifetimeScope != null)
        {
            currentLifetimeScope.Dispose();
        }
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

        releaseManager.RetrieveSavedCredentials();
        if (!ReleaseManager.ProxyTest(releaseManager.Credentials))
        {
            if (ReleaseManager.ProxyTest(CredentialCache.DefaultCredentials))
            {
                releaseManager.Credentials = CredentialCache.DefaultCredentials;
            }
            else if (ReleaseManager.ProxyTest(CredentialCache.DefaultNetworkCredentials))
            {
                releaseManager.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
            else
            {
                var proxySettings = new ProxySettingsView(releaseManager)
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
        await ActivateInstallingViewModel();
    }

    Task ActivateInstallingViewModel()
    {
        ActivateModel<InstallingViewModel>();
        return installer.Install(itemsToInstall);
    }

    public void Handle(InstallSucceededEvent message)
    {
        ActivateModel<SuccessViewModel>();
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
        var messageInfo = string.Format("{0} - {1}", message.Failure, message.FailureDetails);
        raygunClient.Send(new ProductInstallException(messageInfo));
    }


    public void Dispose()
    {
        if (currentLifetimeScope != null)
        {
            currentLifetimeScope.Dispose();
        }
    }
}