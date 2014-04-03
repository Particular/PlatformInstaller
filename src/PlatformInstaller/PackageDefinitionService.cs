using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

public class PackageDefinitionService
{
    readonly PackageManager packageManager;

    public PackageDefinitionService(PackageManager packageManager)
    {
        this.packageManager = packageManager;
    }

    public virtual List<InstallationDefinition> GetPackages()
    {
        return new List<InstallationDefinition>
        {
            new InstallationDefinition
            {
                Name = "NServiceBusPreReqs",
                Image = "/Images/NSB.png",
                PackageDefinitions = new List<PackageDefinition>
                {
                    new PackageDefinition
                    {
                        Name = "NServicebus.PerfCounters.install",
                        DisplayName = "Prerequisites - Custom performance counters"
                    },
                    new PackageDefinition
                    {
                        Name = "NServicebus.Dtc.install",
                        DisplayName = "Prerequisites - Distributed Transaction Coordinator (DTC)"
                    },
                    new PackageDefinition
                    {
                        Name = "NServicebus.Msmq.install",
                        DisplayName = "Default transport - Microsoft Message Queuing (MSMQ)"
                    },
                    new PackageDefinition
                    {
                        Name = "NServicebus.RavenDB.install",
                        DisplayName = "Default storage - RavenDB",
                    },
                },
                SelectedByDefault = !packageManager.IsInstalled("NServicebus.Msmq.install")
                
            },
            new InstallationDefinition
            {
                Name = "ServiceControl",
                Image = "/Images/SC.png",
                PackageDefinitions = new List<PackageDefinition>
                {
                    new PackageDefinition
                    {
                        Name = "ServiceControl.install",
                        DisplayName = "ServiceControl"
                    },
                },
                SelectedByDefault = !packageManager.IsInstalled("ServiceControl.install")
            },
            new InstallationDefinition
            {
                Name = "ServicePulse",
                Image = "/Images/SP.png",
                PackageDefinitions = new List<PackageDefinition>
                {
                    new PackageDefinition
                    {
                        Name = "ServicePulse.install",
                        DisplayName = "ServicePulse"
                    },
                },
                SelectedByDefault = !packageManager.IsInstalled("ServicePulse.install")
            },
            new InstallationDefinition
            {
                Name = "ServiceInsight",
                Image = "/Images/SI.png",
                PackageDefinitions = new List<PackageDefinition>
                {
                    new PackageDefinition
                    {
                        Name = "ServiceInsight.install",
                        DisplayName = "ServiceInsight"
                    },
                },
                SelectedByDefault = !packageManager.IsInstalled("ServiceInsight.install")
            },
           ServiceMatrix.InstallationDefinition(packageManager)
        };
    }
}