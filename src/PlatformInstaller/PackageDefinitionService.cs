using System.Collections.Generic;
using System.IO;

public class PackageDefinitionService
{
    PackageManager packageManager;

    public PackageDefinitionService(PackageManager packageManager)
    {
        this.packageManager = packageManager;
    }

    public virtual List<PackageDefinition> GetPackages()
    {
        return new List<PackageDefinition>
        {
            new PackageDefinition
            {
                Name = "NServiceBusPreReqs",
                Image = "/Images/NSB.png",
                InstallAction = async () =>
                {
                    await packageManager.Install("NServicebus.Dtc.install");
                    await packageManager.Install("NServicebus.PerfCounters.install");
                    await packageManager.Install("NServicebus.Msmq.install");
                    var logFilePath = Path.Combine(PackageManager.GetLogDirectoryForPackage("RavenDB"),"installerlog.txt");
                    if (File.Exists(logFilePath))
                    {
                        File.Delete(logFilePath);
                    }
                    var parmeters = string.Format(@"/quiet /log {0} /msicl RAVEN_TARGET_ENVIRONMENT=DEVELOPMENT /msicl TARGETDIR=C:\ /msicl INSTALLFOLDER=C:\RavenDB /msicl RAVEN_INSTALLATION_TYPE=SERVICE /msicl REMOVE=IIS /msicl ADDLOCAL=Service", logFilePath);
                    await packageManager.Install("RavenDB", parmeters);
                },
            },
            new PackageDefinition
            {
                Name = "ServiceMatrix",
                Image = "/Images/SM.png",
                InstallAction = ()=> packageManager.Install("ServiceMatrix.install"),
                IsInstalledAction = ()=>  packageManager.IsInstalled("ServiceMatrix.install"),
            },
            new PackageDefinition
            {
                Name = "ServiceInsight",
                Image = "/Images/SI.png",
                InstallAction = ()=> packageManager.Install("ServiceInsight.install"),
                IsInstalledAction = ()=> packageManager.IsInstalled("ServiceInsight.install"),
            },
            new PackageDefinition
            {
                Name = "ServicePulse",
                Image = "/Images/SP.png",
                InstallAction = ()=> packageManager.Install("ServicePulse.install"),
                IsInstalledAction = ()=> packageManager.IsInstalled("ServiceControl.install"),
            },
            new PackageDefinition
            {
                Name = "ServiceControl",
                Image = "/Images/SC.png",
                InstallAction = ()=> packageManager.Install("ServiceControl.install"),
                IsInstalledAction = ()=> packageManager.IsInstalled("ServiceControl.install"),
            },
        };
    }
}