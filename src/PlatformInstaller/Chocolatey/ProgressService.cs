using System.Management.Automation;
using PropertyChanged;

[ImplementPropertyChanged]
public class ProgressService
{
    public string OutputText;
    public void OutputDataReceived(PowerShellOutputLine powerShellOutputLine)
    {
        OutputText += powerShellOutputLine.Text;
    }

    public void OutputProgessReceived(ProgressRecord record)
    {
        
    }
}