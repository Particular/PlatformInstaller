using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using NuGet;

public class PackageManager
{
    PowerShellRunner powerShellRunner;
    ChocolateyInstaller chocolateyInstaller;

    public PackageManager(PowerShellRunner powerShellRunner, ChocolateyInstaller chocolateyInstaller)
    {
        this.powerShellRunner = powerShellRunner;
        this.chocolateyInstaller = chocolateyInstaller;
    }


    public Task Uninstall(string packageName, Action<string> logOutput, Action<string> logWarning, Action<string> logError, Action<ProgressRecord> logProgress)
    {
        var parameters = new Dictionary<string, object>
        {
                {"command", "uninstall"},
                {"packageNames", packageName}
        };
        var chocolateyPs1Path = chocolateyInstaller.GetChocolateyPs1Path();
        return powerShellRunner.Run(chocolateyPs1Path, parameters, logOutput, logWarning, logError, logProgress);
    }

    public async Task Install(string packageName, string installArguments, Action<string> logOutput, Action<string> logWarning, Action<string> logError, Action<ProgressRecord> logProgress)
    {
        var parameters = new Dictionary<string, object>
        {
                {"command", "install"},
                {"packageNames", packageName},
                {"source", GetSource()},
                {"verbosity", true},
#if (DEBUG)
                {"pre", true},
#endif
                {"force", true},
                {"y", true}
        };
        if (installArguments != null)
        {
            parameters["installArguments"] = installArguments;
        }
        var chocolateyPs1Path = Path.Combine(chocolateyInstaller.GetInstallPath(), @"chocolateyinstall\chocolatey.ps1");
        Action<string> wrappedLogOutput = s =>
        {
            if (s.ToLower().Contains("reboot is required"))
            {
                logError(s);
                return;
            }

            if (s.ToLower().Contains("the remote name could not be resolved:"))
            {
                logError(s);
                return;
            }


            if (s.ToLower().Contains("unable to find package"))
            {
                logError(s);
                return;
            }


            logOutput(s);

        };
        await powerShellRunner.Run(chocolateyPs1Path, parameters, wrappedLogOutput, logWarning, logError, logProgress);
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

    public bool TryGetInstalledVersion(string packageName, out SemanticVersion version)
    {
        version = null;
        var installPath = chocolateyInstaller.GetInstallPath();
        if (installPath == null)
        {
            return false;
        }
        var chocolateyLibPath = Path.Combine(installPath, "lib");
        if (!Directory.Exists(chocolateyLibPath))
        {
            return false;
        }
        foreach (var directory in Directory.EnumerateDirectories(chocolateyLibPath, packageName + ".*"))
        {
            var versionString = Path.GetFileName(directory).ReplaceCaseless(packageName + ".", "");
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