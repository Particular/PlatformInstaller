using Caliburn.Micro;

public  class DotNetDownloadViewModel : Screen, 
    IHandle<DotNetDownloadProgressEvent>,
    IHandle<DotNetDownloadStartedEvent>,
    IHandle<DotNetDownloadFailedEvent>,
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
    public string Description;

    IEventAggregator eventAggregator;

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }

    public void Handle(DotNetDownloadProgressEvent message)
    {
        PercentComplete = message.ProgressPercentage;
        Description = $"{((int)message.BytesReceived).ToBytesString()} of {((int)message.TotalBytes).ToBytesString()}";
    }
    
    public void Handle(DotNetDownloadStartedEvent message)
    {
        PercentComplete = 0;
    }

    public void Handle(DotNetDownloadCompleteEvent message)
    {
        PercentComplete = 100;
    }

    public void Handle(DotNetDownloadFailedEvent message)
    {
        PercentComplete = 0;
        Description = "Download failed. Retrying...";
    }
}


