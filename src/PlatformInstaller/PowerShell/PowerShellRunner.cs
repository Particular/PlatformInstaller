using System;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Threading.Tasks;
using Anotar.Serilog;

public class PowerShellRunner : IDisposable
{
    Runspace runSpace;
    Pipeline currentPipeline;
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
            const string psExecutionPolicyScript = @"
                    try 
                    {
                        Set-ExecutionPolicy -ExecutionPolicy unrestricted -Scope Process -Force  -ErrorAction SilentlyContinue
                    } 
                    catch {
                    }
                    $global:EffectiveExecutionPolicy = (Get-ExecutionPolicy) -as [string]
            ";

            pipeline.Commands.AddScript(psExecutionPolicyScript);
            pipeline.Invoke();
            var psExecutionPolicy = runSpace.SessionStateProxy.GetVariable("EffectiveExecutionPolicy").ToString();
                       
            var psLockedDownStates = new[]
            {
                "Restricted",
                "AllSigned"
            };
                     
            if (psLockedDownStates.Any(p => p.Equals(psExecutionPolicy, StringComparison.OrdinalIgnoreCase)))
            {
                //TODO: Simon - Can you display this in prettier way, currently this will go to global exception handler - See PI issue #140
                throw new SecurityException("Powershell has been locked down via Group Policy, Platform Installer can not execute scripts");
            }
        }
    }

    public async Task Run(string command, Dictionary<string, object> parameters, Action<string> logOutput, Action<string> logWarning, Action<string> logError, Action<ProgressRecord> logProgress)
    {
        if (currentPipeline !=null)
        {
            throw new Exception("Not thread safe");
        }
        currentPipeline = runSpace.CreatePipeline();
        LogOutput = logOutput.ValueOrDefault();
        LogWarning = logWarning.ValueOrDefault();
        LogError = logError.ValueOrDefault();
        LogProgress = logProgress.ValueOrDefault();

        var psCommand = GetCommand(command, parameters);
        currentPipeline.Commands.Add(psCommand);
        var executableString = psCommand.ToExecutableString();
        logOutput(executableString);
        LogTo.Information("Executing powershell command: " + executableString);
        await TaskEx.Run(() => ExecuteCommand());
    }

    void ExecuteCommand()
    {
        var pipeline = currentPipeline;
        try
        {
            pipeline.Invoke();
            foreach (PSObject errorRecord in pipeline.Error.ReadToEnd())
            {
                var errorMessage = "Error executing powershell: " + errorRecord;
                LogTo.Error(errorMessage);
                LogError(errorMessage);
            }
        }
        finally
        {
            try
            {
                if (pipeline != null)
                {
                    pipeline.Dispose();
                }
            }
            finally
            {
                currentPipeline = null;
            }
        }
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

    public void Abort()
    {
        var pipeline = currentPipeline;
        if (pipeline != null)
        {
            pipeline.Stop();
            pipeline.Dispose();
        }
        currentPipeline = null;
    }
}