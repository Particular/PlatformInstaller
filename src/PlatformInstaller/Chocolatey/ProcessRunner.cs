using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class ProcessRunner
{
    string fileName;
    string arguments;

    public ProcessRunner(string fileName, string arguments)
    {
        this.fileName = fileName;
        this.arguments = arguments;
    }
    
    public Action<string> ErrorDataReceived = error => { };
    public Action<string> OutputDataReceived = output => { };
    

    public Task<int> RunProcessAsync()
    {
        // there is no non-generic TaskCompletionSource
        var tcs = new TaskCompletionSource<int>();

        var process = new Process
        {
            StartInfo =
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardOutput = true,

            },
            EnableRaisingEvents = true,
            
        };
        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null) OutputDataReceived(args.Data);
        };
        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null) ErrorDataReceived(args.Data);
        };
        process.Exited += (sender, args) =>
        {
            tcs.SetResult(process.ExitCode);
            process.Dispose();
        };

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        return tcs.Task;
    }

}