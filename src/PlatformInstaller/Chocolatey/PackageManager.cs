using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;

public class PackageManager
{
    string packageName;

    public Action<PowerShellOutputLine> OutputDataReceived = x => { };
    public Action<PowerShellOutputLine> OutputErrorReceived = x => { };
    public Action<ProgressRecord> OutputProgessReceived = x => { };
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
            OutputProgessReceived = OutputProgessReceived,
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
            OutputProgessReceived = OutputProgessReceived,
            OutputDataReceived = OutputDataReceived,
            OutputErrorReceived = OutputErrorReceived,
        };
        return runner.Run();
    }
}