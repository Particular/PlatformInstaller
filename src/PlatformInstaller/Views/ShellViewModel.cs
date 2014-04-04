using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autofac;
using Caliburn.Micro;

public class ShellViewModel : Conductor<object>,
    IHandle<AgeedToLicenseEvent>,
    IHandle<RunInstallEvent>,
    IHandle<InstallSucceededEvent>,
    IHandle<InstallFailedEvent>,
    IHandle<RebootNeeded>,
    IHandle<ExitApplicationEvent>,
    IHandle<OpenLogDirectoryEvent>,
    IHandle<AgreedToInstallChocolatey>,
    IHandle<HomeEvent>,
    IHandle<UserInstalledChocolatey>,
    IHandle<UserUpdatedChocolatey>
{
    ChocolateyInstaller chocolateyInstaller;
    IEventAggregator eventAggregator;
    List<string> itemsToInstall;
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
            this.ActivateModel<SelectItemsViewModel>();
        }
        else
        {
            this.ActivateModel<LicenseAgreementViewModel>();
        }
    }

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationEvent>();
    }

    public void OpenLogDirectory()
    {
        eventAggregator.Publish<OpenLogDirectoryEvent>();
    }

    public void Handle(AgeedToLicenseEvent message)
    {
        this.ActivateModel<SelectItemsViewModel>();
    }

    public void Handle(OpenLogDirectoryEvent message)
    {
        var a = Logging.LogDirectory;
        Process.Start(Logging.LogDirectory);
    }

    public async void Handle(RunInstallEvent message)
    {
        itemsToInstall = message.SelectedItems;
        if (chocolateyInstaller.IsInstalled())
        {
            var chocolateyUpgradeRequired = await chocolateyInstaller.ChocolateyUpgradeRequired();
            if (chocolateyUpgradeRequired)
            {
                this.ActivateModel<UpdateChocolateyViewModel>();
                return;
            }
            ActivateInstallingViewModel();
            return;
        }
        this.ActivateModel<InstallChocolateyViewModel>();
    }

    void ActivateInstallingViewModel()
    {
        this.ActivateModel<InstallingViewModel>(new NamedParameter("itemsToInstall", itemsToInstall));
    }

    public void Handle(AgreedToInstallChocolatey message)
    {
        this.ActivateModel<InstallingViewModel>(new NamedParameter("itemsToInstall", itemsToInstall));
    }

    public void Handle(UserUpdatedChocolatey message)
    {
        this.ActivateModel<InstallingViewModel>(new NamedParameter("itemsToInstall", itemsToInstall));
    }

    public void Handle(UserInstalledChocolatey message)
    {
        this.ActivateModel<InstallingViewModel>(new NamedParameter("itemsToInstall", itemsToInstall));
    }

    public void Handle(InstallSucceededEvent message)
    {
        this.ActivateModel<SuccessViewModel>();
    }

    public void Handle(RebootNeeded message)
    {
        this.ActivateModel<RebootNeededViewModel>();
    }

    public void Handle(HomeEvent message)
    {
        this.ActivateModel<SelectItemsViewModel>();
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

        this.ActivateModel<FailedInstallationViewModel>(new NamedParameter("failureReason", message.Reason), new NamedParameter("failures", message.Failures));
    }
}