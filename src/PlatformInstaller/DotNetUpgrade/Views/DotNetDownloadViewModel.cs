using Caliburn.Micro;

public  class DotNetDownloadViewModel : Screen, 
    IHandle<DotNetDownloadProgressEvent>,
    IHandle<DotNetDownloadStartedEvent>,
    IHandle<DotNetDownloadCompleteEvent>
{
    

    public DotNetDownloadViewModel()
    {

    }


    public DotNetDownloadViewModel(IEventAggregator eventAggregator)
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

    public void Handle(DotNetDownloadProgressEvent message)
    {
        PercentComplete = message.ProgressPercentage;
    }

    public void Handle(DotNetDownloadStartedEvent message)
    {
        PercentComplete = 0;
    }

    public void Handle(DotNetDownloadCompleteEvent message)
    {
        PercentComplete = 100;
    }
}


