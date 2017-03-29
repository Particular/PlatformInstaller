using System;
using System.Diagnostics;
using System.IO;
using Serilog;
using Serilog.Events;

public static class Logging
{
    public static string LogDirectory;

    public static void Initialise()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        LogDirectory = Path.Combine(appData, AssemblyLocation.ExeFileName);
        Directory.CreateDirectory(LogDirectory);
        const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {SourceContext} {Message:l}{NewLine:l}{Exception:l}";
        var loggingFile = Path.Combine(LogDirectory, "log.txt");
        Log.Logger = new LoggerConfiguration()
            .WriteTo.RollingFile(loggingFile, outputTemplate: template)
            .Filter.ByIncludingOnly(x => !IsEmptyTextMessage(x))
            .CreateLogger();
        var loggingContext = Log.ForContext("SourceContext", "Logging");
        loggingContext.Information($"Starting PlatformInstaller v{VersionFinder.GetVersion()}");
        loggingContext.Information($"Logging to {LogDirectory}");
    }

    public static bool IsEmptyTextMessage(LogEvent logEvent)
    {
        return logEvent.MessageTemplate?.Text?.Trim().Length == 0;
    }

    public static void OpenLogDirectory()
    {
        try
        {
            using (Process.Start("explorer.exe", LogDirectory))
            {
            }
        }
        catch (Exception exception)
        {
            throw new Exception($"Could no open directory {LogDirectory}", exception);
        }
    }

}