using System;
using System.Collections.Generic;
using System.Management.Automation;
using PropertyChanged;

[ImplementPropertyChanged]
public class ProgressService
{
    public string OutputText;

    public ProgressService()
    {
        Failures= new List<LogEntry>();
    }

    public List<LogEntry> Failures { get; set; }

    public void OutputDataReceived(LogEntry logEntry)
    {
        if (logEntry.Type == LogEntryType.Error || logEntry.Type == LogEntryType.Warning)
        {
            Failures.Add(logEntry);
        }

        //hack until we can patch chocolatey
        var outputText = logEntry.Text;
        if (logEntry.Type == LogEntryType.Output && outputText.ToLower().Contains("unable to find package"))
        {
            Failures.Add(logEntry);
        }

        OutputText += outputText;
        if (logEntry.NewLine)
        {
            OutputText += Environment.NewLine;
        }
    }

    public void OutputProgressReceived(ProgressRecord record)
    {
        
    }
}