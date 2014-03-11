using System;
using System.Collections.Generic;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

public class PowerShellRunner : IDisposable
{
    ProgressService progressService;
    Runspace runSpace;

    public PowerShellRunner(ProgressService progressService)
    {
        this.progressService = progressService;
        var host = new PlatformInstallerPSHost(progressService);
        runSpace = RunspaceFactory.CreateRunspace(host);
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
        // Allow scripts to be run regardless of current execution policy
        var psCommand = new Command(command);
        foreach (var commandArg in parameters)
        {
            psCommand.Parameters.Add(commandArg.Key, commandArg.Value);
        }
        pipeline.Commands.Add(psCommand);
        await TaskEx.Run(() => pipeline.Invoke());
    }

    public void Dispose()
    {

    }
}