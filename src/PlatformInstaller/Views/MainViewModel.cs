namespace PlatformInstaller
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using PropertyChanged;
    using System.Windows;
    using System.Collections.Generic;

    [ImplementPropertyChanged]
    public class MainViewModel
    {
        public MainViewModel(ProgressService progressService, 
            PackageDefinitionService packageDefinitionDiscovery, 
            ChocolateyInstaller chocolateyInstaller, 
            WindowManager  windowManager, 
            NewUserDetecter newUserDetecter,
            PackageManager packageManager)
        {
            ProgressService = progressService;

            PackageDefinitionService = packageDefinitionDiscovery;
            this.chocolateyInstaller = chocolateyInstaller;
            this.windowManager = windowManager;
            this.newUserDetecter = newUserDetecter;
            this.packageManager = packageManager;
            packageDefinitions = PackageDefinitionService.GetPackages().SelectMany(p => p.Dependencies.Union(new List<PackageDefinition> { p }));
            packageDefinitions.BindActionToPropChanged(() =>
            {
                SelectedActionCount = packageDefinitions.Count(p => p.Selected);
            }, "Selected");
        }

        public string CurrentPackageDescription;
        public ProgressService ProgressService;
        public PackageDefinitionService PackageDefinitionService;
        ChocolateyInstaller chocolateyInstaller;
        WindowManager windowManager;
        NewUserDetecter newUserDetecter;
        public double SelectedActionCount;
        public bool IsInstallEnabled { get { return SelectedActionCount > 0 && !IsInstalling; } }
        public bool CanClose { get { return !IsInstalling; } }
        public bool IsInstallVisible = true;
        public bool IsFinishedInstalling ;
        public bool InstallFailed;
        public double InstallProgress;
 
        public void Close()
        {
            Application.Current.Shutdown();
        }

        public void BackToProducts()
        {
            InstallProgress = 0;
            IsInstalling = false;
            InstallFailed = false;
            IsInstallVisible = true;
            IsFinishedInstalling = false;
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

            IsInstalling = true;
            InstallCount = SelectedActionCount;

            await InstallChocolatey();

            await InstallPackages(PackageDefinitionService.GetPackages());

            CompleteInstall(newUserDetecter.IsNewUser());
        }

        private async Task InstallPackages(IEnumerable<PackageDefinition> packagesToInstall)
        {
            foreach (var package in packagesToInstall.Where(p => p.Selected))
            {
                CurrentPackageDescription = package.Name;
                if (!string.IsNullOrEmpty(package.ChocolateyPackage))
                {
                    await packageManager.Install(package.ChocolateyPackage);
                    await InstallPackages(package.Dependencies);
                }
                InstallProgress++;
            }
        }

        private void CompleteInstall(bool isNewUser)
        {
            IsInstallVisible = false;
            if (!ProgressService.Failures.Any())
            {
                Process.Start(@"http://particular.net/thank-you-for-downloading-the-particular-service-platform?new_user=" + isNewUser.ToString().ToLower());
                IsFinishedInstalling = true;
            }
            else
            {
                InstallFailed = true;
            }

            IsInstalling = false;
        }

        private async Task InstallChocolatey()
        {
            if (!chocolateyInstaller.IsInstalled())
            {
                InstallCount++;
                await chocolateyInstaller.InstallChocolatey();
                InstallProgress++;
            }
        }

        public double InstallCount;

        public bool IsInstalling;
        private PackageManager packageManager;
        IEnumerable<PackageDefinition> packageDefinitions;
    }
}