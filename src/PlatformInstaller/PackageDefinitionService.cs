using System.Collections.Generic;

public class PackageDefinitionService
{
    public List<PackageDefinition> Packages;

    public PackageDefinitionService(PackageManager packageManager)
    {
        Packages = new List<PackageDefinition>
        {
            new PackageDefinition
            {
                Name = "Service Matrix",
                Image = "/Images/SM.png",
                Install = () => packageManager.Install("ServiceMatrix.install")
            },
            new PackageDefinition
            {
                Name = "Raven",
                Image = "/Images/RavenDb.png",
                Install = () => packageManager.Install("RavenDB")
            },
            new PackageDefinition
            {
                Name = "Service Insight",
                Image = "/Images/SI.png",
                Install = () => packageManager.Install("ServiceInsight.install")
            },
            new PackageDefinition
            {
                Name = "Service Pulse",
                Image = "/Images/SP.png",
                Install = () => packageManager.Install("ServicePulse.install")
            },
            new PackageDefinition
            {
                Name = "Service Control",
                Image = "/Images/SC.png",
                Install = () => packageManager.Install("ServiceControl.install") 
            },
            new PackageDefinition
            {
                Name = "Service Bus",
                Image = "/Images/NSB.png",
                Install = async () =>
                {
                   await packageManager.Install("NServicebus.Dtc.install");
                   await packageManager.Install("NServicebus.PerfCounters.install");
                   await packageManager.Install("NServicebus.Msmq.install");
                   await packageManager.Install("RavenDB");
                   await packageManager.Install("RavenDB");
                }
            }
        };


    }
}