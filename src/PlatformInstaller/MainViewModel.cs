using System;
using System.Linq;
using System.Threading.Tasks;
using PropertyChanged;
using System.Collections.Generic;
using System.Windows;

namespace PlatformInstaller
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        public MainViewModel(PackageManager packageManager, ProgressService progressService, IPackageDiscoveryService packageDiscovery)
        {
            this.progressService = progressService;
            
            this.packageManager = packageManager;
            this.packageManager.InstallStarted += packageManager_InstallStarted;
            this.packageManager.InstallComplete += packageManager_InstallComplete;

            this.packageDiscovery = packageDiscovery;

            this.Step = 0;
        }

        void packageManager_InstallStarted(object sender, InstallStartedEventArgs e)
        {
            this.CurrentPackageDescription = e.PackageDescription;
        }

        void packageManager_InstallComplete(object sender, InstallCompleteEventArgs e)
        {
            InstallProgress++;
            if (InstallProgress == InstallCount)
            {
                Task.Delay(100);
                Step = 2;
            }
        }

        public string CurrentPackageDescription { get; set; }

        private ProgressService progressService;
        private PackageManager packageManager;
        private IPackageDiscoveryService packageDiscovery;
        private IEnumerable<IPackage> products;

        public double InstallCount { get; private set; }
        public double InstallProgress { get; private set; }
        public int Step { get; private set; }

        public IEnumerable<IPackage> Products
        {
            get
            {
                if (this.products == null)
                {
                    products = Flatten(packageDiscovery.GetServices());
                }
                return products;
            }
        }

        private IEnumerable<IPackage> Flatten(IEnumerable<IPackage> products, bool withAutomatic = false)
        {
            if (products == null)
                return null;

            return products.Where(p => withAutomatic || !p.Automatic).Concat(products.SelectMany(e => Flatten(e.Children) ?? new List<IPackage>()));
        }

        public void Close()
        {
            Application.Current.Shutdown();
        }

        public async Task InstallSelected()
        {
            var toInstall = products.Where(p => p.Selected).SelectMany(p => new List<IPackage> { p }.Union(p.Children.Where(c => c.Automatic)));
            InstallCount = toInstall.Count();
            Step = 1;

            foreach (var package in toInstall)
            {
                await packageManager.Install(package.Chocolatey, string.Format("Installing {0}", package.Name));
            }
        }
    }
}