namespace PlatformInstaller
{
    using System.Diagnostics;
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
        NewUserDetecter newUserDetecter;
        IEventAggregator eventAggregator;

        public ShellViewModel(IEventAggregator eventAggregator, ChocolateyInstaller chocolateyInstaller, IWindowManager windowManager, NewUserDetecter newUserDetecter)
        {
            this.chocolateyInstaller = chocolateyInstaller;
            this.windowManager = windowManager;
            this.newUserDetecter = newUserDetecter;
            this.eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            this.ActivateModel<LicenseAgreementViewModel>();
        }

        public void Close()
        {
            base.TryClose();
        }

        public void OpenLogDirectory()
        {
            eventAggregator.Publish<OpenLogDirectoryEvent>();
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

            this.ActivateModel<InstallingViewModel>(x => x.ItemsToInstall = message.SelectedItems);
        }

        public void Handle(InstallSucceededEvent message)
        {
            Process.Start(@"http://particular.net/thank-you-for-downloading-the-particular-service-platform?new_user=" + newUserDetecter.IsNewUser().ToString().ToLower());
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

        public string Version
        {
            get { return "Version " + GetType().Assembly.GetName().Version.ToString(3); }
        }
    }
}

