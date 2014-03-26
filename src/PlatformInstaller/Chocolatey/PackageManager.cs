using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet;

public class PackageManager
{
    PowerShellRunner powerShellRunner;

    public PackageManager(PowerShellRunner powerShellRunner)
    {
        this.powerShellRunner = powerShellRunner;
    }

    public async Task Install(string packageName, string installerParmeters = null)
    {
        var parameters = new Dictionary<string, object>
        {
                {"command", "install"},
                {"packageNames", packageName},
                {"source", GetSource()},
                {"verbosity", true},
                {"pre", true}
        };
        if (installerParmeters != null)
        {
            parameters["installArguments"] = installerParmeters;
        }
        await powerShellRunner.Run(@"C:\Chocolatey\chocolateyinstall\chocolatey.ps1", parameters);
        CopyLogFiles(packageName);
    }

    public static void CopyLogFiles(string packageName)
    {
        var packageTempDirectory = Path.Combine(Path.GetTempPath(), "Chocolatey", packageName);
        if (!Directory.Exists(packageTempDirectory))
        {
            return;
        }
        var logFiles = Directory.GetFiles(packageTempDirectory, "*.log");
        if (!logFiles.Any())
        {
            return;
        }
        var targetDirectory = GetLogDirectoryForPackage(packageName);
        foreach (var logFile in logFiles)
        {
            FileEx.CopyToDirectory(logFile, targetDirectory);
        }
    }

    public static string GetLogDirectoryForPackage(string packageName)
    {
        var targetDirectory = Path.Combine(Logging.LogDirectory, packageName + "PackageLogs");
        Directory.CreateDirectory(targetDirectory);
        return targetDirectory;
    }


    static string GetSource()
    {
#if (DEBUG)
        return @"C:\ChocolateyResourceCache;http://chocolatey.org/api/v2";
#else
        return "http://chocolatey.org/api/v2";
#endif
    }

    public Task Uninstall(string packageName)
    {
        var parameters = new Dictionary<string, object>
        {
                {"command", "uninstall"},
                {"packageNames", packageName}
        };
        return powerShellRunner.Run(@"C:\Chocolatey\chocolateyinstall\chocolatey.ps1", parameters);
    }
    public bool TryGetInstalledVersion(string packageName, out SemanticVersion version)
    {
        version = null;
        if (!Directory.Exists(@"C:\Chocolatey\lib"))
        {
            return false;
        }
        foreach (var directory in Directory.EnumerateDirectories(@"C:\Chocolatey\lib", packageName + ".*"))
        {
            var versionString = Path.GetFileName(directory).ReplaceCaseless(packageName +".","");
            SemanticVersion newVersion;
            if (SemanticVersion.TryParse(versionString, out newVersion))
            {
                if (version == null || newVersion > version)
                {
                    version = newVersion;
                }
            }
        }
        return version != null;
    }
    public bool IsInstalled(string packageName)
    {
        SemanticVersion version;
        return TryGetInstalledVersion(packageName, out version);
    }
}