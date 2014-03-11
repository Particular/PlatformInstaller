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
                    new InstallPackage { Name = "Service Matrix", Image = "/Images/SM.png", Chocolatey = "ServiceMatrix.install" },
                    new InstallPackage { Name = "Service Control", Image = "/Images/SC.png", Chocolatey = "ServiceControl.install",
                        Children = new List<InstallPackage> 
                                    { 
                                        new InstallPackage { Name = "Service Insight", Image = "/Images/SI.png", Chocolatey = "ServiceInsight.install" },
                                        new InstallPackage { Name = "Service Pulse", Image = "/Images/SP.png", Chocolatey = "ServicePulse.install" },
                                        new InstallPackage { Name = "MSMQ", Automatic = true, Chocolatey = "NServicebus.Msmq.install" },
                                        new InstallPackage { Name = "DTC", Automatic = true, Chocolatey = "NServicebus.Dtc.install" },
                                        new InstallPackage { Name = "Raven", Automatic = true, Chocolatey = "RavenDB" },
                                        new InstallPackage { Name = "PerfCounters", Automatic = true, Chocolatey = "NServicebus.PerfCounters.install" },
                                    }
                    },
                    new InstallPackage { Name = "Service Bus", Image = "/Images/NSB.png", Chocolatey = "NServicebus.install" }
                };
            }

            return packages;
        }
    }
}
