﻿using System;
using System.Threading.Tasks;

public class ChocolateyInstaller
{
    ProcessRunner processRunner;

    public ChocolateyInstaller(ProcessRunner  processRunner)
    {
        this.processRunner = processRunner;

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
}
