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
                InstallAction = () => packageManager.Install("ServiceMatrix.install"),
                Installed =  packageManager.IsInstalled("ServiceMatrix.install")
            },
            new PackageDefinition
            {
                Name = "Raven",
                Image = "/Images/RavenDb.png",
                InstallAction = () => packageManager.Install("RavenDB"),
                Installed =  packageManager.IsInstalled("RavenDB")
            },
            new PackageDefinition
            {
                Name = "Service Insight",
                Image = "/Images/SI.png",
                InstallAction = () => packageManager.Install("ServiceInsight.install"),
                Installed =  packageManager.IsInstalled("ServiceInsight.install")
            },
            new PackageDefinition
            {
                Name = "Service Pulse",
                Image = "/Images/SP.png",
                InstallAction = () => packageManager.Install("ServicePulse.install"),
                Installed =  packageManager.IsInstalled("ServicePulse.install")
            },
            new PackageDefinition
            {
                Name = "Service Control",
                Image = "/Images/SC.png",
                InstallAction = () => packageManager.Install("ServiceControl.install"),
                Installed =  packageManager.IsInstalled("ServiceControl.install")
            },
            new PackageDefinition
            {
                Name = "Service Bus",
                Image = "/Images/NSB.png",
                InstallAction = async () =>
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