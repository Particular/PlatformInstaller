using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Anotar.Serilog;

public class PowerShellRunner : IDisposable
{
    Runspace runSpace;
    Pipeline pipeLine;
    PipelineReader<PSObject> outPut;
    bool hasData;
    public event Action<string> OutputChanged = x => { };
    public event Action RunFinished = () => { };

    public PowerShellRunner()
    {
        runSpace = RunspaceFactory.CreateRunspace();
        runSpace.Open();
    }

    public void Run(String command)
    {
        LogTo.Information("Running command: {0}", command);
        pipeLine = runSpace.CreatePipeline(command);
        pipeLine.Input.Close();
        outPut = pipeLine.Output;
        outPut.DataReady += (x, y) => OutputDataReady();
        pipeLine.InvokeAsync();
    }

    public void OnOutputChanged(string output)
    {
        LogTo.Debug("Output changed: {0}", output);
        OutputChanged(output);
    }


    void OutputDataReady()
    {
        LogTo.Debug("Output ready");
        var data = pipeLine.Output.NonBlockingRead();
        if (data.Count > 0)
        {
            LogTo.Debug("Has data");
            hasData = true;
            foreach (var d in data)
            {
                LogTo.Debug("data: {0}", d);
                OnOutputChanged(d + Environment.NewLine);
            }

        }
        if (!pipeLine.Output.EndOfPipeline)
        {
            return;
        }
        if (!hasData)
        {
            LogTo.Debug("No output");
            OnOutputChanged("No output");
        }
        LogTo.Debug("Run finished");
        RunFinished();
    }

    public void Dispose()
    {

    }
}