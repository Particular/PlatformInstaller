using System.Windows;
using Caliburn.Micro;

public class GroupPolicyErrorViewModel : Screen
{
    IEventAggregator eventAggregator;
    PowerShellRunner powerShellRunner;

    public GroupPolicyErrorViewModel()
    {
        
    }
    public GroupPolicyErrorViewModel(IEventAggregator eventAggregator, PowerShellRunner powerShellRunner)
    {
        // ReSharper disable once DoNotCallOverridableMethodsInConstructor
        DisplayName = "Group Policy Error";
        ListGpoCommand = "Get-ExecutionPolicy -List";
        OverrideCommand = "Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope LocalMachine";
        this.eventAggregator = eventAggregator;
        this.powerShellRunner = powerShellRunner;
    }

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }

    public void ReCheck()
    {
        if (powerShellRunner.TrySetExecutionPolicyToUnrestricted())
        {
            eventAggregator.Publish<UserFixedExecutionPolicy>();
        }
    }


    public string OverrideCommand { get; set; }
    public string ListGpoCommand { get; set; }
    
    public void CopyListGpoCommand()
    {
        Clipboard.SetText(ListGpoCommand);
    }
    public void CopyOverrideCommand()
    {
        Clipboard.SetText(OverrideCommand);
    }

}