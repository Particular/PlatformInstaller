using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Anotar.Serilog;

public class ProcessRunner
{
    public Task<int> RunProcess(string fileName, string arguments, Action<string> outputAction, Action<string> errorAction)
    {
        LogTo.Information(string.Format("Executing process: {0} {1}", fileName, arguments));
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
                RedirectStandardOutput = true
            },
            EnableRaisingEvents = true,
        };
        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                LogTo.Information(args.Data);
                outputAction(args.Data);
            }
        };
        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                LogTo.Error(args.Data);
                errorAction(args.Data);
            }
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