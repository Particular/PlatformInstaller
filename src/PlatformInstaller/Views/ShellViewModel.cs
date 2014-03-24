using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;

public class ShellViewModel : Conductor<object>,
    IHandle<AgeedToLicenseEvent>,
    IHandle<RunInstallEvent>,
    IHandle<InstallSucceededEvent>,
    IHandle<CloseApplicationEvent>,
    IHandle<OpenLogDirectoryEvent>,
    IHandle<AgreedToInstallChocolatey>,
    IHandle<HomeEvent>
{
    ChocolateyInstaller chocolateyInstaller;
    IEventAggregator eventAggregator;
    public string Version = "v" + typeof(ShellViewModel).Assembly.GetName().Version.ToString(3);
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
    
    public void Close()
    {
        Application.Current.Shutdown();
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
        Process.Start(Logging.LogDirectory);
    }

    public void Handle(RunInstallEvent message)
    {
        itemsToInstall = message.SelectedItems;
        if (chocolateyInstaller.IsInstalled())
        {
            this.ActivateModel<InstallingViewModel>(x => x.ItemsToInstall = message.SelectedItems);
        }
        else
        {
            this.ActivateModel<InstallChocolateyViewModel>();
        }
    }

    public void Handle(AgreedToInstallChocolatey message)
    {
        this.ActivateModel<InstallingViewModel>(x => x.ItemsToInstall = itemsToInstall);
    }

    public void Handle(InstallSucceededEvent message)
    {
        this.ActivateModel<SuccessViewModel>();
    }

    public void Handle(HomeEvent message)
    {
        this.ActivateModel<SelectItemsViewModel>();
    }

    public void Handle(CloseApplicationEvent message)
    {
        base.TryClose();
    }

}