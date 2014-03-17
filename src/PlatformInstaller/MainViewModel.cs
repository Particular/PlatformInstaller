namespace PlatformInstaller
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using PropertyChanged;
    using System.Windows;
    using Tests;

    [ImplementPropertyChanged]
    public class MainViewModel
    {
        public MainViewModel(ProgressService progressService, PackageDefinitionService packageDefinitionDiscovery, ChocolateyInstaller chocolateyInstaller)
        {
            ProgressService = progressService;

            PackageDefinitionService = packageDefinitionDiscovery;
            this.chocolateyInstaller = chocolateyInstaller;
            PackageDefinitionService.Packages.BindActionToPropChanged(() =>
            {
                SelectedActionCount = PackageDefinitionService.Packages.Count(p => p.Selected);
            }, "Selected");
        }

        public string CurrentPackageDescription;
        public ProgressService ProgressService;
        public PackageDefinitionService PackageDefinitionService;
        ChocolateyInstaller chocolateyInstaller;
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
                var messageBoxResult = MessageBox.Show("The Platform Installer has a dependency on Chocolatey. Click OK to install it or install it yourself.", "Chocolatey required", MessageBoxButton.OKCancel);
                if (messageBoxResult == MessageBoxResult.Cancel)
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
            Process.Start(@"http://particular.net/thank-you-for-downloading-the-particular-service-platform?new_user=" + NewUserDetecter.Current.IsNewUser().ToString().ToLower());

            IsInstalling = false;
            IsFinishedInstalling = true;
        }

        public double InstallCount;

        public bool IsInstalling;
    }

}


