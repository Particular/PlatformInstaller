using Caliburn.Micro;

public class RebootNeededViewModel : Screen
{
    public RebootNeededViewModel(IEventAggregator eventAggregator)
    {
        DisplayName = "Reboot Required";
        this.eventAggregator = eventAggregator;
    }

    IEventAggregator eventAggregator;
    
    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationEvent>();
    }
    public void Reboot()
    {
        eventAggregator.Publish<RebootMachineEvent>();
        eventAggregator.Publish<ExitApplicationEvent>();
    }

}