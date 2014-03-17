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
        if (logEntry.Type == LogEntryType.Output && logEntry.Text.ToLower().Contains("unable to find package"))
        {
            Failures.Add(logEntry);
        }

        OutputText += logEntry.Text;
    }

    public void OutputProgessReceived(ProgressRecord record)
    {
        
    }
}