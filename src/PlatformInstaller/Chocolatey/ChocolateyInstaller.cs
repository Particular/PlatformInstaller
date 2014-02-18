using System.Diagnostics;
using System.Threading.Tasks;

public class ChocolateyInstaller
{
    public Task<int> InstallChocolatey()
    {
        var processRunner = new ProcessRunner("cmd.exe",
            @"/c @powershell -NoProfile -ExecutionPolicy unrestricted -Command ""iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))"" && SET PATH=%PATH%;%systemdrive%\chocolatey\bin")
        {
            OutputDataReceived = x => Debug.WriteLine(x),
            ErrorDataReceived = x => Debug.WriteLine(x)
        };
        return processRunner.RunProcessAsync();
    }
}