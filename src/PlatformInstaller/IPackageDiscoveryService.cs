using System;
using System.Collections.Generic;
namespace PlatformInstaller
{
    public interface IPackageDiscoveryService
    {
        IEnumerable<IPackage> GetServices();
    }
}
