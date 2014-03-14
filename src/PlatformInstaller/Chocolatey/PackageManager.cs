using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class PackageManager
{
    PowerShellRunner powerShellRunner;

    public event InstallStartedDelegate InstallStarted = (sender, args) => {};

    public event InstallCompleteDelegate InstallComplete = (sender, args) => { };

    public event InstallProgressDelegate InstallProgress = (sender, args) => { };

    public PackageManager(PowerShellRunner powerShellRunner)
    {
        this.powerShellRunner = powerShellRunner;
    }

    public async Task Install(string packageName, string packageDesctiption)
    {
        InstallStarted(this, new InstallStartedEventArgs { PackageName = packageName, PackageDescription = packageDesctiption });
        InstallProgress(this, new InstallProgressEventArgs { PackageName = packageName, Log = "Starting...", Progress = 0 });
        await Task.Delay(5000); //for now
        //var parameters = new Dictionary<string, object>
        //{
        //        {"command", "install"},
        //        {"packageNames", packageName},
        //        {"source", @"C:\ChocolateyResourceCache;http://chocolatey.org/api/v2"},
        //        {"verbosity", true},
        //        {"pre", true}
        //};
        //await powerShellRunner.Run(@"C:\Chocolatey\chocolateyinstall\chocolatey.ps1", parameters);
        InstallComplete(this, new InstallCompleteEventArgs { PackageName = packageName });
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
    public bool TryGetInstalledVersion(string packageName, out Version version)
    {
        version = null;
        foreach (var directory in Directory.EnumerateDirectories(@"C:\Chocolatey\lib", packageName + ".*"))
        {
            var versionString = Path.GetFileName(directory).ReplaceCaseless(packageName +".","");
            Version newVersion;
            if (Version.TryParse(versionString, out newVersion))
            {
                if (version == null || newVersion > version)
                {
                    version = newVersion;
                }
            }
        }
        return version != null;
    }
}

public delegate void InstallStartedDelegate(object sender, InstallStartedEventArgs e);
public delegate void InstallCompleteDelegate(object sender, InstallCompleteEventArgs e);
public delegate void InstallProgressDelegate(object sender, InstallProgressEventArgs e);

public class InstallStartedEventArgs : EventArgs
{
    public string PackageName { get; set; }
    public string PackageDescription { get; set; }
}

public class InstallCompleteEventArgs : EventArgs
{
    public string PackageName { get; set; }
}

public class InstallProgressEventArgs : EventArgs
{
    public string PackageName { get; set; }
    public double Progress { get; set; }
    public string Log { get; set; }
}