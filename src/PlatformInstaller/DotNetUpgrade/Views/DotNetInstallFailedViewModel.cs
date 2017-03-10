using Caliburn.Micro;

public class DotNetInstallFailedViewModel : Screen
{
    IEventAggregator eventAggregator;

    public DotNetInstallFailedViewModel()
    {

    }
    public DotNetInstallFailedViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
    }

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }
}