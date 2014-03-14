using System.Diagnostics;
using System.Threading.Tasks;

public class ProcessRunner
{
    ProgressService progressService;

    public ProcessRunner(ProgressService progressService)
    {
        this.progressService = progressService;
    }

    public Task<int> RunProcessAsync(string fileName, string arguments)
    {
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
            if (args.Data != null)
            {
                progressService.OutputDataReceived(new LogEntry(args.Data,LogEntryType.Output));
            }
        };
        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                progressService.OutputDataReceived(new LogEntry(args.Data, LogEntryType.Error));
            }
        };
        process.Exited += (sender, args) =>
        {
            tcs.SetResult(process.ExitCode);
            //TODO: result
            process.Dispose();
        };

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        return tcs.Task;
    }

}