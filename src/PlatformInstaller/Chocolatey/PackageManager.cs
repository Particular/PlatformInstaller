using System;
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
        var runner = new PowerShellRunner("cinst " + packageName)
        {
            OutputDataReceived = OutputDataReceived,
            OutputErrorReceived = OutputErrorReceived,
        };
        return runner.Run();
    }

    public Task Uninstall()
    {
        var runner = new PowerShellRunner("cuninst " + packageName)
        {
            OutputDataReceived = OutputDataReceived,
            OutputErrorReceived = OutputErrorReceived,
        };
        return runner.Run();
    }
}