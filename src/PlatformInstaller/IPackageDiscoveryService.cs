using System.Collections.Generic;

public interface IPackageDiscoveryService
{
    IEnumerable<InstallPackage> GetServices();
}
