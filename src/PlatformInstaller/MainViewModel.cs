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
        public MainViewModel(ProgressService progressService, PackageDefinitionService packageDefinitionDiscovery)
        {
            ProgressService = progressService;

            PackageDefinitionService = packageDefinitionDiscovery;
            PackageDefinitionService.Packages.BindActionToPropChanged(() =>
            {
                InstallCount = PackageDefinitionService.Packages.Count(p => p.Selected);
            }, "Selected");
        }

        public string CurrentPackageDescription;
        public ProgressService ProgressService;
        public PackageDefinitionService PackageDefinitionService;
        public double InstallCount;
        public bool IsInstallEnabled{get { return InstallCount > 0; }}
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
            IsInstallVisible = false;
            IsInstalling = true;
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

        public bool IsInstalling;
    }
}


