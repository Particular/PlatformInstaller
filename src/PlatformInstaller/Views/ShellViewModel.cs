namespace PlatformInstaller
{
    using Caliburn.Micro;

    public class ShellViewModel : Conductor<object>,
        IHandle<AgeedToLicenseEvent>,
        IHandle<RunInstallEvent>,
        IHandle<InstallSucceededEvent>,
        IHandle<CloseApplicationEvent>,
        IHandle<HomeEvent>
    {
        ChocolateyInstaller chocolateyInstaller;
        IWindowManager windowManager;

        public ShellViewModel(IEventAggregator eventAggregator, ChocolateyInstaller chocolateyInstaller, IWindowManager windowManager)
        {
            this.chocolateyInstaller = chocolateyInstaller;
            this.windowManager = windowManager;
            eventAggregator.Subscribe(this);
            this.ActivateModel<LicenseAgreementViewModel>(); 
        }

        public void Cancel()
        {
            base.TryClose();
        }

        public void Handle(AgeedToLicenseEvent message)
        {
            this.ActivateModel<SelectItemsViewModel>();
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

            this.ActivateModel<InstallingViewModel>(x=>x.ItemsToInstall = message.SelectedItems);
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
}

