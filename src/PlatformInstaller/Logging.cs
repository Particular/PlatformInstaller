using System;
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
        var loggingFile = Path.Combine(LogDirectory, "log.txt");
        Log.Logger = new LoggerConfiguration()
            .WriteTo.RollingFile(loggingFile)
            .Filter.ByIncludingOnly(x => !IsEmptyTextMessage(x))
            .CreateLogger();
    }

    static bool IsEmptyTextMessage(LogEvent logEvent)
    {
        if (logEvent.MessageTemplate == null)
        {
            return false;
        }
        if (logEvent.MessageTemplate.Text == null)
        {
            return false;
        }
        return logEvent.MessageTemplate.Text.Trim().Length == 0;
    }



}