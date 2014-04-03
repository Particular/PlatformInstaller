using System.Collections.Generic;
using System.Linq;
using PlatformInstaller;

public class ServiceMatrix
{

    public static InstallationDefinition InstallationDefinition(PackageManager packageManager)
    {
        var versionsToInstall = VisualStudioDetecter.InstalledVersions.Where(version => supportedVersions.Contains(version));

        
        var installationDefinition = new InstallationDefinition
        {
            Name = "ServiceMatrix",
            Image = "/Images/SM.png",
            Disabled = true,
            ToolTip = "ServiceMatrix requires Visual Studio 2012 to be installed,",
            PackageDefinitions = new List<PackageDefinition>(),
            SelectedByDefault = false
        };

        
        foreach (var version in versionsToInstall)
        {
            var packageName = "ServiceMatrix." + version + ".install";

            if (packageManager.IsInstalled(packageName))
            {
                continue;
            }

            installationDefinition.PackageDefinitions.Add(new PackageDefinition
                 {
                     Name =packageName,
                     DisplayName = "ServiceMatrix for " + version
                 });
        }

        if (installationDefinition.PackageDefinitions.Any())
        {
            installationDefinition.Status = "Install";
            installationDefinition.SelectedByDefault = true;
            installationDefinition.Disabled = false;
            installationDefinition.ToolTip = "Install ServiceMatrix";
        }
        else
        {
            if (versionsToInstall.Any())
            {
                installationDefinition.Status = "Already installed";
                installationDefinition.ToolTip = "ServiceMatrix for Visual Studio 2012";
            }
            else
            {
                installationDefinition.Status = "VS 2012 required";
                installationDefinition.ToolTip = "ServiceMatrix requires Visual Studio 2012";
            }
        }

        return installationDefinition;
    }

    static List<string> supportedVersions = new List<string> { "VS2012" };
}