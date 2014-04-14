using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Caliburn.Micro;
using Janitor;

[SkipWeaving]
public class ShellViewModel : Conductor<object>,
    IDisposable,
    IHandle<AgeedToLicenseEvent>,
    IHandle<RunInstallEvent>,
    IHandle<InstallSucceededEvent>,
    IHandle<InstallFailedEvent>,
    IHandle<RebootNeeded>,
    IHandle<ExitApplicationEvent>,
    IHandle<AgreedToInstallChocolatey>,
    IHandle<HomeEvent>,
    IHandle<UserInstalledChocolatey>,
    IHandle<UserUpdatedChocolatey>
{
    ChocolateyInstaller chocolateyInstaller;
    IEventAggregator eventAggregator;
    List<string> itemsToInstall;
    ILifetimeScope currentLifetimeScope;
    public static Screen StartModel;

    public ShellViewModel(IEventAggregator eventAggregator, ChocolateyInstaller chocolateyInstaller, LicenseAgreement licenseAgreement)
    {
        this.chocolateyInstaller = chocolateyInstaller;
        this.eventAggregator = eventAggregator;
        if (StartModel != null)
        {
            base.ActivateItem(StartModel);
            return;
        }
        if (licenseAgreement.HasAgreedToLicense())
        {
            ActivateModel<SelectItemsViewModel>();
        }
        else
        {
            ActivateModel<LicenseAgreementViewModel>();
        }
    }

    public void ActivateModel<T>(params Autofac.Core.Parameter[] parameters) where T : Screen
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
        currentLifetimeScope = ContainerFactory.Container.BeginLifetimeScope();
        var model = currentLifetimeScope.Resolve<T>(parameters);
        if (model.IsHandler())
        {
            eventAggregator.Subscribe(model);
        }
        ActivateItem(model);
    }

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationEvent>();
    }

    public void Handle(AgeedToLicenseEvent message)
    {
        ActivateModel<SelectItemsViewModel>();
    }

    public async void Handle(RunInstallEvent message)
    {
        itemsToInstall = message.SelectedItems;
        if (chocolateyInstaller.IsInstalled())
        {
            var chocolateyUpgradeRequired = await chocolateyInstaller.ChocolateyUpgradeRequired();
            if (chocolateyUpgradeRequired)
            {
                ActivateModel<UpdateChocolateyViewModel>();
                return;
            }
            ActivateInstallingViewModel();
            return;
        }
        ActivateModel<InstallChocolateyViewModel>();
    }

    void ActivateInstallingViewModel()
    {
        ActivateModel<InstallingViewModel>(new NamedParameter("itemsToInstall", itemsToInstall));
    }

    public void Handle(AgreedToInstallChocolatey message)
    {
        ActivateModel<InstallingViewModel>(new NamedParameter("itemsToInstall", itemsToInstall));
    }

    public void Handle(UserUpdatedChocolatey message)
    {
        ActivateModel<InstallingViewModel>(new NamedParameter("itemsToInstall", itemsToInstall));
    }

    public void Handle(UserInstalledChocolatey message)
    {
        ActivateModel<InstallingViewModel>(new NamedParameter("itemsToInstall", itemsToInstall));
    }

    public void Handle(InstallSucceededEvent message)
    {
        ActivateModel<SuccessViewModel>();
    }

    public void Handle(RebootNeeded message)
    {
        ActivateModel<RebootNeededViewModel>();
    }

    public void Handle(HomeEvent message)
    {
        ActivateModel<SelectItemsViewModel>();
    }

    public void Handle(ExitApplicationEvent message)
    {
        base.TryClose();
    }

    public void Handle(InstallFailedEvent message)
    {
        var reboot = message.Failures.FirstOrDefault(f => f.Contains("reboot is required"));
        if (reboot != null)
        {
            eventAggregator.Publish<RebootNeeded>();
            return;
        }

        ActivateModel<FailedInstallationViewModel>(new NamedParameter("failureReason", message.Reason), new NamedParameter("failures", message.Failures));
    }

    public void Dispose()
    {
        if (currentLifetimeScope != null)
        {
            currentLifetimeScope.Dispose();
        }
    }
}