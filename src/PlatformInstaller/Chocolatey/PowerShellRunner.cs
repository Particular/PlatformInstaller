using System;
using System.Diagnostics;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

public class PowerShellRunner : IDisposable
{
    string command;
    Runspace runSpace;
    Pipeline pipeLine;
    public Action<string> OutputDataReceived = x => { };
    public Action<string> OutputErrorReceived = x => { };
    TaskCompletionSource<object> completionSource;

    public PowerShellRunner(string command)
    {
        this.command = command;
    }

    public Task Run()
    {
        completionSource = new TaskCompletionSource<object>();
        runSpace = RunspaceFactory.CreateRunspace();
        runSpace.Open();
        pipeLine = runSpace.CreatePipeline(command);
        pipeLine.Input.Close();
        pipeLine.Output.DataReady += (x, y) => ReadOutput();
        pipeLine.Error.DataReady += (x, y) => ReadError();
        pipeLine.StateChanged += pipeLine_StateChanged;
        pipeLine.InvokeAsync();
        return completionSource.Task;
    }

    void pipeLine_StateChanged(object sender, PipelineStateEventArgs e)
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
        foreach (var output in pipeLine.Output.NonBlockingRead())
        {
            OutputDataReceived(output.ToString());
        }
    }

    void ReadError()
    {
        foreach (var error in pipeLine.Error.NonBlockingRead())
        {
            OutputErrorReceived(error.ToString());
        }
    }

    public void Dispose()
    {

    }
}