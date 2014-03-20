using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;

public class ShellViewModel : Conductor<object>,
    IHandle<AgeedToLicenseEvent>,
    IHandle<RunInstallEvent>,
    IHandle<InstallSucceededEvent>,
    IHandle<CloseApplicationEvent>,
        IHandle<OpenLogDirectoryEvent>,
    IHandle<HomeEvent>
{
    ChocolateyInstaller chocolateyInstaller;
    IWindowManager windowManager;
    IEventAggregator eventAggregator;

    public ShellViewModel(IEventAggregator eventAggregator, ChocolateyInstaller chocolateyInstaller, IWindowManager windowManager, LicenseAgreement licenseAgreement)
    {
        this.chocolateyInstaller = chocolateyInstaller;
        this.windowManager = windowManager;
        this.eventAggregator = eventAggregator;
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
        if (!chocolateyInstaller.IsInstalled())
        {
            if (!windowManager.ShowDialog<InstallChocolateyViewModel>().UserChoseToContinue)
            {
                return;
            }
        }

        this.ActivateModel<InstallingViewModel>(x => x.ItemsToInstall = message.SelectedItems);
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

    public string Version = "v" + typeof(ShellViewModel).Assembly.GetName().Version.ToString(3);
}
