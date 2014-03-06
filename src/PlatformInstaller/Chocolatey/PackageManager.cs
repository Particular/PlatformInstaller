using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PackageManager
{
    string packageName;

    public Action<string> OutputDataReceived = x => { };
    public Action<string> OutputErrorReceived = x => { };
    public PackageManager(string packageName)
    {
        this.packageName = packageName;
    }

    public Task Install()
    {
        var parameters = new Dictionary<string, object>
        {
                {"command", "install"},
                {"packageNames", packageName},
                {"source", @"C:\ChocolateyResourceCache;http://chocolatey.org/api/v2"},
                {"verbosity", true},
                {"pre", true}
        };
        var runner = new PowerShellRunner(@"C:\Chocolatey\chocolateyinstall\chocolatey.ps1" , parameters)
        {
            OutputDataReceived = OutputDataReceived,
            OutputErrorReceived = OutputErrorReceived,
        };
        return runner.Run();
    }

    public Task Uninstall()
    {
        var parameters = new Dictionary<string, object>
        {
                {"command", "uninstall"},
                {"packageNames", packageName}
        };
        var runner = new PowerShellRunner(@"C:\Chocolatey\chocolateyinstall\chocolatey.ps1" , parameters)
        {
            OutputDataReceived = OutputDataReceived,
            OutputErrorReceived = OutputErrorReceived,
        };
        return runner.Run();
    }
}