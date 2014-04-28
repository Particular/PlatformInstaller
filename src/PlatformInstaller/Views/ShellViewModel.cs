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
    IHandle<UserUpdatedChocolatey>,
    IHandle<UserFixedExecutionPolicy>
{
    ChocolateyInstaller chocolateyInstaller;
    LicenseAgreement licenseAgreement;
    ILifetimeScope lifetimeScope;
    PowerShellRunner powerShellRunner;
    IEventAggregator eventAggregator;
    List<string> itemsToInstall;
    ILifetimeScope currentLifetimeScope;

    public ShellViewModel(IEventAggregator eventAggregator, ChocolateyInstaller chocolateyInstaller, LicenseAgreement licenseAgreement, ILifetimeScope lifetimeScope, PowerShellRunner powerShellRunner)
    {
        this.chocolateyInstaller = chocolateyInstaller;
        this.licenseAgreement = licenseAgreement;
        this.lifetimeScope = lifetimeScope;
        this.powerShellRunner = powerShellRunner;
        this.eventAggregator = eventAggregator;
        RunStartupSequence();
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
        eventAggregator.Publish<ExitApplicationEvent>();
    }

    public void Handle(AgeedToLicenseEvent message)
    {
        RunStartupSequence();
    }

    public void Handle(UserFixedExecutionPolicy message)
    {
        RunStartupSequence();
    }

    void RunStartupSequence()
    {
        if (!licenseAgreement.HasAgreedToLicense())
        {
            ActivateModel<LicenseAgreementViewModel>();
            return;
        }
        if (!powerShellRunner.TrySetExecutionPolicyToUnrestricted())
        {
            ActivateModel<GroupPollicyErrorViewModel>();
            return;
        }
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