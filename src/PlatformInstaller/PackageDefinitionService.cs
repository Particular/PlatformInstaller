using System.Collections.Generic;

public class PackageDefinitionService
{
    
    public virtual List<PackageDefinition> GetPackages()
    {
        return new List<PackageDefinition>
        {
            new PackageDefinition
            {
                Name = "Service Bus",
                Image = "/Images/NSB.png",
                Dependencies = new List<PackageDefinition>
                {
                    new PackageDefinition{ Name = "DTC", ChocolateyPackage = "NServicebus.Dtc.install" },
                    new PackageDefinition{ Name = "Performance Counters", ChocolateyPackage = "NServicebus.PerfCounters.install" },
                    new PackageDefinition{ Name = "MSMQ", ChocolateyPackage = "NServicebus.Msmq.install" },
                    new PackageDefinition{ Name = "Raven", ChocolateyPackage = "RavenDB", Image = "/Images/RavenDB.png" },
                }
            },
            new PackageDefinition
            {
                Name = "Service Matrix",
                Image = "/Images/SM.png",
                ChocolateyPackage = "ServiceMatrix.install"
            },
            new PackageDefinition
            {
                Name = "Service Insight",
                Image = "/Images/SI.png",
                ChocolateyPackage = "ServiceInsight.install"
            },
            new PackageDefinition
            {
                Name = "Service Pulse",
                Image = "/Images/SP.png",
                ChocolateyPackage = "ServicePulse.install"
            },
            new PackageDefinition
            {
                Name = "Service Control",
                Image = "/Images/SC.png",
                ChocolateyPackage = "ServiceControl.install"
            },
        };
    }
}