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

            Step = 0;
        }

        public string CurrentPackageDescription;
        public ProgressService ProgressService;
        public PackageDefinitionService PackageDefinitionService;
        public double InstallCount;
        public double InstallProgress;
        public int Step;
 
        public void Close()
        {
            Application.Current.Shutdown();
        }

        public async Task InstallSelected()
        {
            var toInstall = PackageDefinitionService.Packages.Where(p => p.Selected).ToList();
            InstallCount = toInstall.Count();
            Step = 1;

            foreach (var package in toInstall)
            {
                CurrentPackageDescription = package.Name;
                await package.InstallAction();
                InstallProgress++;
            }
            Step = 2;
            Process.Start(@"http://particular.net/thank-you-for-downloading-the-particular-service-platform?new_user=" + NewUserDetecter.Current.IsNewUser().ToString().ToLower());
        }

    }
}


