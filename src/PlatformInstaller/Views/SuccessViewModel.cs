using Caliburn.Micro;

public class SuccessViewModel : Screen
{
    public SuccessViewModel()
    {
        
    }
    public SuccessViewModel(IEventAggregator eventAggregator)
    {
        // ReSharper disable once DoNotCallOverridableMethodsInConstructor
        DisplayName = "Success";
        this.eventAggregator = eventAggregator;
    }

    IEventAggregator eventAggregator;

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }

    public void Home()
    {
        eventAggregator.Publish<NavigateHomeCommand>();
    }

}