using System;
using System.Diagnostics;
using System.IO;
using Caliburn.Micro;
using Serilog;
using Serilog.Events;

public class Logging:
        IHandle<OpenLogDirectoryEvent>
{
    string logDirectory;

    public Logging()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        logDirectory = Path.Combine(appData, "PlatformInstaller");
        Directory.CreateDirectory(logDirectory);
        var loggingFile = Path.Combine(logDirectory, "log.txt");
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


    public void Handle(OpenLogDirectoryEvent message)
    {
        Process.Start(logDirectory);
    }

}