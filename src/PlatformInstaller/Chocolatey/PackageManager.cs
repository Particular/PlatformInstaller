using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PackageManager
{
    PowerShellRunner powerShellRunner;

    public PackageManager(PowerShellRunner powerShellRunner)
    {
        this.powerShellRunner = powerShellRunner;
    }

    public Task Install(string packageName)
    {
        var parameters = new Dictionary<string, object>
        {
                {"command", "install"},
                {"packageNames", packageName},
                {"source", @"C:\ChocolateyResourceCache;http://chocolatey.org/api/v2"},
                {"verbosity", true},
                {"pre", true}
        };
        return powerShellRunner.Run(@"C:\Chocolatey\chocolateyinstall\chocolatey.ps1", parameters);
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