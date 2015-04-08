using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;

public class PackageManager
{
    ChocolateyInstaller chocolateyInstaller;
    ProcessRunner processRunner;

    public PackageManager(ProcessRunner processRunner,ChocolateyInstaller chocolateyInstaller)
    {
        this.processRunner = processRunner;
        this.chocolateyInstaller = chocolateyInstaller;
    }

    public Task Uninstall(string packageName, Action<string> logOutput, Action<string> logError)
    {
        var chocolateyExePath = Path.Combine(chocolateyInstaller.GetInstallPath(), @"bin\chocolatey.exe");
        var parameters = new StringBuilder();
        parameters.AppendFormat(" uninstall {0}", packageName);
        parameters.Append(" --confirm");
        parameters.Append(" --force");
        return processRunner.RunProcess(chocolateyExePath, parameters.ToString(), logOutput, logError);
    }
    
    public async Task Install(string packageName, string installArguments, Action<string> logOutput, Action<string> logError)
    {

        var outputLog  = new List<string>();
        Action<string> wrappedLogOutput = s => {
             outputLog.Add(s);
             logOutput(s);
        };

        var chocolateyExePath = Path.Combine(chocolateyInstaller.GetInstallPath(), @"bin\chocolatey.exe");
        var parameters = new StringBuilder();
        parameters.AppendFormat(" install {0}", packageName);
        parameters.AppendFormat(" --source=\"{0}\"", GetSource());
        parameters.Append(" --confirm");
        parameters.Append(" --force");
#if (DEBUG)
        parameters.Append(" --prerelease");
#endif

        if (installArguments != null)
        {
            parameters.AppendFormat(" --params=\"{0}\"", installArguments);
        }
        var exitCode = await processRunner.RunProcess(chocolateyExePath, parameters.ToString(), wrappedLogOutput, logError);
        if (exitCode != 0)
        {
            foreach (var s in outputLog)
            {
                logError(s);                      
            }
        }
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
        var processStartInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(chocolateyInstaller.GetInstallPath(), @"bin\chocolatey.exe"),
            Arguments = string.Format("list {0} -l -r", packageName),
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };
        var process = Process.Start(processStartInfo);
        if (process != null)
        {
            process.WaitForExit();
            var output = process.StandardOutput.ReadToEnd();
            if (output.Contains("|"))
            {
                return SemanticVersion.TryParse(output.Split("|".ToCharArray()).Last(), out version);
            }
        }
        version = null;
        return false;
    }

    public bool IsInstalled(string packageName)
    {
        SemanticVersion version;
        return TryGetInstalledVersion(packageName, out version);
    }
}