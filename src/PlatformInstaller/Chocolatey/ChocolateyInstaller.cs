using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class ChocolateyInstaller
{
    ProcessRunner processRunner;
    PowerShellRunner powerShellRunner;

    public ChocolateyInstaller(ProcessRunner processRunner, PowerShellRunner powerShellRunner)
    {
        this.processRunner = processRunner;
        this.powerShellRunner = powerShellRunner;
    }

    public string GetChocolateyPs1Path()
    {

        return Path.Combine(GetInstallPath(), @"chocolateyinstall\chocolatey.ps1");
    }

    public Task<int> InstallChocolatey(Action<string> outputAction, Action<string> errorAction)
    {
        outputAction = outputAction.ValueOrDefault();
        errorAction = errorAction.ValueOrDefault();
        var arguments = @"/c @powershell -NoProfile -ExecutionPolicy unrestricted -Command ""iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))"" && SET PATH=%PATH%;%systemdrive%\chocolatey\bin";
        return processRunner.RunProcess("cmd.exe", arguments, outputAction, errorAction);
    }

    public bool IsInstalled()
    {
        return GetInstallPath() != null;
    }

    public string GetInstallPath()
    {
        return Environment.GetEnvironmentVariable("ChocolateyInstall");
    }

    public async Task<Version> GetInstallVersion()
    {
        var output = new List<string>();
        await powerShellRunner.Run(GetChocolateyPs1Path(), null, output.Add, null, null, null);
        var lineWithVersion = output.FirstOrDefault(x => x.Contains("chocolatey v"));
        if (lineWithVersion == null)
        {
            return null;
        }
        var versionPart = lineWithVersion.Split(new[]
        {
            "chocolatey v"
        }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        if (versionPart == null)
        {
            return null;
        }
        Version version;
        Version.TryParse(versionPart, out version);
        return version;
    }
}
