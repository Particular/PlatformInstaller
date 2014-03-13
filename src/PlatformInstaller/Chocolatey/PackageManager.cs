using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PackageManager
{
    PowerShellRunner powerShellRunner;

    public event InstallStartedDelegate InstallStarted;

    public event InstallCompleteDelegate InstallComplete;

    public event InstallProgressDelegate InstallProgress;

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