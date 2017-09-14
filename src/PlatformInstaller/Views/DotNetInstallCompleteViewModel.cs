using Caliburn.Micro;

public class DotNetInstallCompleteViewModel : Screen
{

    public DotNetInstallCompleteViewModel()
    {

    }

    public DotNetInstallCompleteViewModel(IEventAggregator eventAggregator)
    {
        DisplayName = "Install Complete";
        this.eventAggregator = eventAggregator;
    }

    IEventAggregator eventAggregator;

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }

}