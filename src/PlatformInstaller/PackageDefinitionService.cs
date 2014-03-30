using System.Collections.Generic;
using System.IO;

public class PackageDefinitionService
{

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
                        Name = "NServicebus.Dtc.install"
                    },
                    new PackageDefinition
                    {
                        Name = "NServicebus.PerfCounters.install"
                    },
                    new PackageDefinition
                    {
                        Name = "NServicebus.Msmq.install"
                    },
                    new PackageDefinition
                    {
                        Name = "RavenDB",
                        Parameters = GetRavenDbParameters()
                    },
                }
            },
            new InstallationDefinition
            {
                Name = "ServiceMatrix",
                Image = "/Images/SM.png",
                PackageDefinitions = new List<PackageDefinition>
                {
                    new PackageDefinition
                    {
                        Name = "ServiceMatrix.install"
                    },
                }
            },
            new InstallationDefinition
            {
                Name = "ServiceInsight",
                Image = "/Images/SI.png",
                PackageDefinitions = new List<PackageDefinition>
                {
                    new PackageDefinition
                    {
                        Name = "ServiceInsight.install"
                    },
                }
            },
            new InstallationDefinition
            {
                Name = "ServicePulse",
                Image = "/Images/SP.png",
                PackageDefinitions = new List<PackageDefinition>
                {
                    new PackageDefinition
                    {
                        Name = "ServicePulse.install"
                    },
                }
            },
            new InstallationDefinition
            {
                Name = "ServiceControl",
                Image = "/Images/SC.png",
                PackageDefinitions = new List<PackageDefinition>
                {
                    new PackageDefinition
                    {
                        Name = "ServiceControl.install"
                    },
                }
            },
        };
    }

    static string GetRavenDbParameters()
    {
        var logFilePath = Path.Combine(PackageManager.GetLogDirectoryForPackage("RavenDB"), "installerlog.txt");
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }
        return string.Format(@"/quiet /log {0} /msicl RAVEN_TARGET_ENVIRONMENT=DEVELOPMENT /msicl TARGETDIR=C:\ /msicl INSTALLFOLDER=C:\RavenDB /msicl RAVEN_INSTALLATION_TYPE=SERVICE /msicl REMOVE=IIS /msicl ADDLOCAL=Service", logFilePath);
    }
}