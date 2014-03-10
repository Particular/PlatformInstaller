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
        public string OutputText { get; set; }
        public bool CanInstall = true;
        private IPackageDiscoveryService packageDiscovery;
        private IEnumerable<IPackage> products;

        public MainViewModel(IPackageDiscoveryService packageDiscovery)
        {
            this.packageDiscovery = packageDiscovery;
        }

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

        private IEnumerable<IPackage> Flatten(IEnumerable<IPackage> products)
        {
            if (products == null)
                return null;

            return products.Concat(products.SelectMany(e => Flatten(e.Children) ?? new List<IPackage>()));
        }

        public void Close()
        {
            Application.Current.Shutdown();
        }

        async Task InstallPackage(string packageName)
        {
            OutputText = "";
            CanInstall = false;
            var packageInstaller = new PackageManager(packageName)
            {
                OutputDataReceived = s => { OutputText += s + Environment.NewLine; }
            };
            await packageInstaller.Install();
            CanInstall = true;
        }

    }
}