using System;
using System.Collections.Generic;
using System.Linq;
using PlatformInstaller;

public class ServiceMatrix
{

    public static InstallationDefinition InstallationDefinition(PackageManager packageManager, string visualStudioVersion)
    {
        var isVisualStudioInstalled = VisualStudioDetecter.InstalledVersions.Contains(visualStudioVersion);
        var packageName = string.Format("ServiceMatrix.{0}.install", visualStudioVersion);
        var isChocolateyPackageInstalled = packageManager.IsInstalled(packageName);
        return GetInstallationDefinition(visualStudioVersion, isVisualStudioInstalled, isChocolateyPackageInstalled, packageName);
    }

    public static InstallationDefinition GetInstallationDefinition(string visualStudioVersion, bool isVisualStudioInstalled, bool isChocolateyPackageInstalled, string packageName)
    {
        string iconToUse;
        switch (visualStudioVersion)
        {
            case VisualStudioVersions.VS2013:
                iconToUse = "/Images/SM2013.png";
                break;
            case VisualStudioVersions.VS2012:
                iconToUse = "/Images/SM2012.png";
                break;
            default:
                throw new Exception(string.Format("Specified VisualStudio version: {0} is not supported", visualStudioVersion));
        }

        var installationDefinition = new InstallationDefinition
        {
            SortOrder = 20,
            Name = "ServiceMatrix",
            Image = iconToUse,
            Disabled = true,
            PackageDefinitions = new List<PackageDefinition>(),
            SelectedByDefault = false
        };

        if (!isVisualStudioInstalled)
        {
            installationDefinition.Status = string.Format("{0} Required", visualStudioVersion);
            installationDefinition.ToolTip = string.Format("This option requires {0}", visualStudioVersion);
            return installationDefinition;
        }

        if (!isChocolateyPackageInstalled)
        {
            installationDefinition.PackageDefinitions.Add(new PackageDefinition
            {
                Name = packageName,
                DisplayName = "ServiceMatrix for " + visualStudioVersion
            });
            installationDefinition.Status = "Install";
            installationDefinition.SelectedByDefault = true;
            installationDefinition.Disabled = false;
            installationDefinition.ToolTip = string.Format("Install ServiceMatrix for {0}", visualStudioVersion);
        }
        else
        {
            installationDefinition.Status = "Already Installed";
            installationDefinition.ToolTip = string.Format("ServiceMatrix for {0} is already installed", visualStudioVersion);
        }
        return installationDefinition;
    }

    static List<string> supportedVersions = new List<string> { "VS2012", "VS2013" };
}