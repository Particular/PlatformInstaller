using Caliburn.Micro;

public class RebootNeededViewModel : Screen
{
    public RebootNeededViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "Reboot Required";
    }

    IEventAggregator eventAggregator;

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationEvent>();
    }
}