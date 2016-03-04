using Caliburn.Micro;

public  class DotNetPreReqViewModel : Screen
{
    public DotNetPreReqViewModel()
    {

    }
    public DotNetPreReqViewModel(IEventAggregator eventAggregator)
    {
        DisplayName = "Prerequisites";
        this.eventAggregator = eventAggregator;
    }

    IEventAggregator eventAggregator;

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }

    public void Install()
    {
        eventAggregator.Publish<DotNetStartInstallWizardCommand>();
    }
}


