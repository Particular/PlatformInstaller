using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Anotar.Serilog;

public class PowerShellRunner : IDisposable
{
    Runspace runSpace;
    bool isRunning;
    public Action<string> LogOutput;
    public Action<string> LogWarning;
    public Action<string> LogError;
    public Action<ProgressRecord> LogProgress;

    public PowerShellRunner()
    {
        var psHost = new PSHost(new PSHostUserInterface(this)); 
        runSpace = RunspaceFactory.CreateRunspace(psHost);
        runSpace.Open();
        // Allows scripts to be run for this process
        using (var pipeline = runSpace.CreatePipeline())
        {
            pipeline.Commands.AddScript("Set-ExecutionPolicy -ExecutionPolicy unrestricted -Scope Process -Force");
            pipeline.Invoke();
        }
    }

    public async Task Run(string command, Dictionary<string, object> parameters, Action<string> logOutput, Action<string> logWarning, Action<string> logError, Action<ProgressRecord> logProgress)
    {
        if (isRunning)
        {
            throw new Exception("No thread safe");
        }
        isRunning = true;
        LogOutput = logOutput.ValueOrDefault();
        LogWarning = logWarning.ValueOrDefault();
        LogError = logError.ValueOrDefault();
        LogProgress = logProgress.ValueOrDefault();

        var pipeline = runSpace.CreatePipeline();
        var psCommand = GetCommand(command, parameters);
        pipeline.Commands.Add(psCommand);
        var executableString = psCommand.ToExecutableString();
        logOutput(executableString);
        LogTo.Information("Executing powershell command: " + executableString);
        await TaskEx.Run(() =>
        {
            try
            {
                pipeline.Invoke();
                foreach (PSObject errorRecord in pipeline.Error.ReadToEnd())
                {
                    var errorMessage = "Error executing powershell: " + errorRecord;
                    LogTo.Error(errorMessage);
                    logError(errorMessage);
                }
            }
            finally
            {
                isRunning = false;
            }
        });
    }

    static Command GetCommand(string command, Dictionary<string, object> parameters)
    {
        var psCommand = new Command(command);
        if (parameters != null)
        {
            foreach (var commandArg in parameters)
            {
                psCommand.Parameters.Add(commandArg.Key, commandArg.Value);
            }
        }
        return psCommand;
    }

    public void Dispose()
    {

    }
}