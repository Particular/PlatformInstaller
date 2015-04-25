using System.Collections.Generic;

public class PackageDefinitionService
{
    PackageManager packageManager;

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
                SortOrder = 10,
                Name = "NServiceBusPreReqs",
                Image = "/Images/NSB.png",
                PackageDefinitions = new List<PackageDefinition>
                {
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
                },
                SelectedByDefault = !packageManager.IsInstalled("NServicebus.Msmq.install")
                
            },
            new InstallationDefinition
            {
                SortOrder = 50,
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
                SortOrder = 40,
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
                SortOrder = 30,
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
            ServiceMatrix.InstallationDefinition(packageManager, VisualStudioVersions.VS2013),
            ServiceMatrix.InstallationDefinition(packageManager, VisualStudioVersions.VS2012)
        };
    }
}