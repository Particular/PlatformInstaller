using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

public class PowerShellRunner : IDisposable
{
    string command;
    Dictionary<string, object> parameters;
    Runspace runSpace;
    Pipeline pipeline;
    public Action<string> OutputDataReceived = x => { };
    public Action<string> OutputErrorReceived = x => { };
    TaskCompletionSource<object> completionSource;

    public PowerShellRunner(string command, Dictionary<string,object> parameters)
    {
        this.command = command;
        this.parameters = parameters;
    }

    public Task Run()
    {
        completionSource = new TaskCompletionSource<object>();
        runSpace = RunspaceFactory.CreateRunspace();
        runSpace.Open();
        pipeline = runSpace.CreatePipeline();

        var psCommand = new Command(command);
        foreach (var commandArg in parameters)
        {
            psCommand.Parameters.Add(commandArg.Key, commandArg.Value);
        }
        pipeline.Commands.Add(psCommand);
        pipeline.Input.Close();
        pipeline.Output.DataReady += (x, y) => ReadOutput();
        pipeline.Error.DataReady += (x, y) => ReadError();
        pipeline.StateChanged += PipelineStateChanged;
        pipeline.Invoke();
        return completionSource.Task;
    }

    void PipelineStateChanged(object sender, PipelineStateEventArgs e)
    {
        if (e.PipelineStateInfo.Reason != null)
        {
            Debug.WriteLine(e.PipelineStateInfo.Reason);
            completionSource.TrySetException(e.PipelineStateInfo.Reason);
            return;
        }
        if (e.PipelineStateInfo.State == PipelineState.Completed)
        {
            completionSource.TrySetResult(null);
        }
    }


    void ReadOutput()
    {
        foreach (var output in pipeline.Output.NonBlockingRead())
        {
            OutputDataReceived(output.ToString());
        }
    }

    void ReadError()
    {
        foreach (var error in pipeline.Error.NonBlockingRead())
        {
            OutputErrorReceived(error.ToString());
        }
    }

    public void Dispose()
    {

    }
}