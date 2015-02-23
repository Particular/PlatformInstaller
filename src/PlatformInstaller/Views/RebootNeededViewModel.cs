using Caliburn.Micro;

public class RebootNeededViewModel : Screen
{
    public RebootNeededViewModel()
    {
        
    }
    public RebootNeededViewModel(IEventAggregator eventAggregator)
    {
        DisplayName = "Reboot Required";
        this.eventAggregator = eventAggregator;
    }

    IEventAggregator eventAggregator;
    
    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }
    public void Reboot()
    {
        eventAggregator.Publish<RebootMachineCommand>();
        eventAggregator.Publish<ExitApplicationCommand>();
    }

}