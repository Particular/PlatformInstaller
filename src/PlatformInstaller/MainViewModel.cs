namespace PlatformInstaller
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using PropertyChanged;
    using System.Windows;

    [ImplementPropertyChanged]
    public class MainViewModel
    {
        public MainViewModel(ProgressService progressService, PackageDefinitionService packageDefinitionDiscovery, ChocolateyInstaller chocolateyInstaller, WindowManager  windowManager, NewUserDetecter newUserDetecter)
        {
            ProgressService = progressService;

            PackageDefinitionService = packageDefinitionDiscovery;
            this.chocolateyInstaller = chocolateyInstaller;
            this.windowManager = windowManager;
            this.newUserDetecter = newUserDetecter;
            PackageDefinitionService.Packages.BindActionToPropChanged(() =>
            {
                SelectedActionCount = PackageDefinitionService.Packages.Count(p => p.Selected);
            }, "Selected");
        }

        public string CurrentPackageDescription;
        public ProgressService ProgressService;
        public PackageDefinitionService PackageDefinitionService;
        ChocolateyInstaller chocolateyInstaller;
        WindowManager windowManager;
        NewUserDetecter newUserDetecter;
        public double SelectedActionCount;
        public bool IsInstallEnabled { get { return SelectedActionCount > 0; } }
        public bool CanClose { get { return !IsInstalling; } }
        public bool IsInstallVisible = true;
        public bool IsFinishedInstalling ;
        public double InstallProgress;
 
        public void Close()
        {
            Application.Current.Shutdown();
        }

        public async Task InstallSelected()
        {
            if (!chocolateyInstaller.IsInstalled())
            {
                if (!windowManager.ShowDialog<InstallChocolateyViewModel>().UserChoseToContinue)
                {
                    return;
                }
            }

            IsInstallVisible = false;
            IsInstalling = true;
            InstallCount = SelectedActionCount;
            if (!chocolateyInstaller.IsInstalled())
            {
                InstallCount++;
                await chocolateyInstaller.InstallChocolatey();
                InstallProgress++;
            }
            foreach (var package in PackageDefinitionService.Packages.Where(p => p.Selected))
            {
                CurrentPackageDescription = package.Name;
                await package.InstallAction();
                InstallProgress++;
            }
            Process.Start(@"http://particular.net/thank-you-for-downloading-the-particular-service-platform?new_user=" + newUserDetecter.IsNewUser().ToString().ToLower());

            IsInstalling = false;
            IsFinishedInstalling = true;
        }

        public double InstallCount;

        public bool IsInstalling;
    }
}