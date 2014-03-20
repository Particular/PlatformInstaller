using System;
using System.Collections.Generic;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Anotar.Serilog;

public class PowerShellRunner : IDisposable
{
    ProgressService progressService;
    Runspace runSpace;

    public PowerShellRunner(PlatformInstallerPSHost platformInstallerPsHost, ProgressService progressService)
    {
        this.progressService = progressService;
        runSpace = RunspaceFactory.CreateRunspace(platformInstallerPsHost);
        runSpace.Open();
        // Allows scripts to be run for this process
        using (var pipeline = runSpace.CreatePipeline())
        {
            pipeline.Commands.AddScript("Set-ExecutionPolicy -ExecutionPolicy unrestricted -Scope Process -Force");
            pipeline.Invoke();
        }
    }

    public async Task Run(string command, Dictionary<string, object> parameters)
    {
        var pipeline = runSpace.CreatePipeline();
        var psCommand = new Command(command);
        foreach (var commandArg in parameters)
        {
            psCommand.Parameters.Add(commandArg.Key, commandArg.Value);
        }
        pipeline.Commands.Add(psCommand);
        var executableString = psCommand.ToExecutableString();
        progressService.OutputDataReceived(new LogEntry(executableString,LogEntryType.Output));
        LogTo.Information("Executing powershell command: " + executableString);
        await TaskEx.Run(() => pipeline.Invoke());
    }

    public void Dispose()
    {

    }
}