using System.Windows;
using Caliburn.Micro;

public class GroupPollicyErrorViewModel : Screen
{
    IEventAggregator eventAggregator;
    PowerShellRunner powerShellRunner;

    public GroupPollicyErrorViewModel(IEventAggregator eventAggregator, PowerShellRunner powerShellRunner)
    {
        this.eventAggregator = eventAggregator;
        this.powerShellRunner = powerShellRunner;
    }

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationEvent>();
    }

    public void ReCheck()
    {
        if (powerShellRunner.TrySetExecutionPolicyToUnrestricted())
        {
            eventAggregator.Publish<UserFixedExecutionPolicy>();
        }
    }


    public string OverrideCommand = "Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope LocalMachine";
    public string ListGpoCommand = "Get-ExecutionPolicy -List";
    
    public void CopyListGpoCommand()
    {
        Clipboard.SetText(ListGpoCommand);
    }
    public void CopyOverrideCommand()
    {
        Clipboard.SetText(OverrideCommand);
    }

}