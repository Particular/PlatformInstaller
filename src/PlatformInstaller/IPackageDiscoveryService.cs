using System.Collections.Generic;

public interface IPackageDiscoveryService
{
    IEnumerable<IPackage> GetServices();
}
