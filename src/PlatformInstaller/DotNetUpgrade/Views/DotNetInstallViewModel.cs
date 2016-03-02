using Caliburn.Micro;

public  class DotNetInstallViewModel : Screen
{
    public DotNetInstallViewModel()
    {

    }
    
    public DotNetInstallViewModel(IEventAggregator eventAggregator)
    {
        DisplayName = "Downloading";
        this.eventAggregator = eventAggregator;
    }

    public int PercentComplete { get; set; }

    IEventAggregator eventAggregator;

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }
}


