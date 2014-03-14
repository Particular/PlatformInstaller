namespace PlatformInstaller
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using PropertyChanged;
    using System.Collections.Generic;
    using System.Windows;
    using Tests;

    [ImplementPropertyChanged]
    public class MainViewModel
    {
        public MainViewModel(PackageManager packageManager, ProgressService progressService, IPackageDiscoveryService packageDiscovery)
        {
            this.progressService = progressService;

            this.packageManager = packageManager;
            this.packageManager.InstallStarted += packageManager_InstallStarted;
            this.packageManager.InstallComplete += packageManager_InstallComplete;
            this.packageManager.InstallProgress += packageManager_InstallProgress;

            this.packageDiscovery = packageDiscovery;

            Step = 0;
            Log = string.Empty;
        }

        void packageManager_InstallProgress(object sender, InstallProgressEventArgs e)
        {
            Log += string.Format("{0}\n", e.Log);
        }

        void packageManager_InstallStarted(object sender, InstallStartedEventArgs e)
        {
            CurrentPackageDescription = e.PackageDescription;
        }

        void packageManager_InstallComplete(object sender, InstallCompleteEventArgs e)
        {
            InstallProgress++;
            if (InstallProgress == InstallCount)
            {
                Task.Delay(100);
                Step = 2;
                Process.Start(@"http://particular.net/thank-you-for-downloading-the-particular-service-platform?new_user=" + NewUserDetecter.Current.IsNewUser().ToString().ToLower());
            }
        }

        public string CurrentPackageDescription;

        ProgressService progressService;
        PackageManager packageManager;
        IPackageDiscoveryService packageDiscovery;
        IEnumerable<InstallPackage> products;

        public double InstallCount;
        public double InstallProgress;
        public int Step;
        public string Log;
 
        public IEnumerable<InstallPackage> Products
        {
            get
            {
                if (products == null)
                {
                    products = Flatten(packageDiscovery.GetServices());
                }
                return products;
            }
        }

        IEnumerable<InstallPackage> Flatten(IEnumerable<InstallPackage> products, bool withAutomatic = false)
        {
            if (products == null)
                return null;

            return products.Where(p => withAutomatic || !p.Automatic).Concat(products.SelectMany(e => Flatten(e.Children) ?? new List<InstallPackage>()));
        }

        public void Close()
        {
            Application.Current.Shutdown();
        }

        public async Task InstallSelected()
        {
            var toInstall = products.Where(p => p.Selected).SelectMany(p => new List<InstallPackage>
            {
                p
            }.Union(p.Children.Where(c => c.Automatic)));
            InstallCount = toInstall.Count();
            Step = 1;

            foreach (var package in toInstall)
            {
                if (package.Enabled)
                {
                    await packageManager.Install(package.Chocolatey, string.Format("Installing {0}", package.Name));
                }
            }
        }

    }
}