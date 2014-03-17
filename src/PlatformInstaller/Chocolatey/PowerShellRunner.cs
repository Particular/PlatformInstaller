using System;
using System.Collections.Generic;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

public class PowerShellRunner : IDisposable
{
    Runspace runSpace;

    public PowerShellRunner(PlatformInstallerPSHost platformInstallerPsHost)
    {
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
        await TaskEx.Run(() => pipeline.Invoke());
    }

    public void Dispose()
    {

    }
}