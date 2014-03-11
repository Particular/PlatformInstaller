using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformInstaller
{
    class HardcodedPackageService : IPackageDiscoveryService
    {
        IEnumerable<IPackage> packages;

        public IEnumerable<IPackage> GetServices()
        {
            if (packages == null)
            {
                packages = new List<InstallPackage>
                {
                    new InstallPackage { Name = "Service Matrix", Image = "/Images/SM.png" },
                    new InstallPackage { Name = "Service Control", Image = "/Images/SC.png", Children = new List<InstallPackage> 
                                                                    { 
                                                                        new InstallPackage { Name = "Service Insight", Image = "/Images/SI.png" },
                                                                        new InstallPackage { Name = "Service Pulse", Image = "/Images/SP.png" } 
                                                                    }
                    },
                    new InstallPackage { Name = "Service Bus", Image = "/Images/NSB.png" }
                };
            }

            return packages;
        }
    }
}
