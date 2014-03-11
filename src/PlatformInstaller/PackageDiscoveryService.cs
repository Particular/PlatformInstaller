using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformInstaller
{
    class PackageDiscoveryService : PlatformInstaller.IPackageDiscoveryService
    {
        public IEnumerable<IPackage> GetServices()
        {
            return null;
        }
    }
}
