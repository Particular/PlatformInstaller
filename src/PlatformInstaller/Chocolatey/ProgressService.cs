using System.Management.Automation;
using PropertyChanged;

[ImplementPropertyChanged]
public class ProgressService
{
    public string OutputText;
    public void OutputDataReceived(LogEntry logEntry)
    {
        OutputText += logEntry.Text;
    }

    public void OutputProgessReceived(ProgressRecord record)
    {
        
    }
}