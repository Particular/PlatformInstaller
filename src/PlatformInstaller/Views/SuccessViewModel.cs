using Caliburn.Micro;

public class SuccessViewModel : Screen
{
    public SuccessViewModel(IEventAggregator eventAggregator)
    {
        DisplayName = "Success";
        this.eventAggregator = eventAggregator;
    }

    IEventAggregator eventAggregator;

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationEvent>();
    }

    public void Home()
    {
        eventAggregator.Publish<HomeEvent>();
    }

}