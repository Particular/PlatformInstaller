using System;
using System.Threading.Tasks;

public class ChocolateyInstaller
{
    readonly ProcessRunner processRunner;

    public ChocolateyInstaller(ProcessRunner  processRunner)
    {
        this.processRunner = processRunner;
    }

    public Task<int> InstallChocolatey()
    {
        var arguments = @"/c @powershell -NoProfile -ExecutionPolicy unrestricted -Command ""iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))"" && SET PATH=%PATH%;%systemdrive%\chocolatey\bin";
        return processRunner.RunProcessAsync("cmd.exe", arguments);
    }

    public bool IsInstalled()
    {
        return Environment.GetEnvironmentVariable("ChocolateyInstall") != null;
    }
}