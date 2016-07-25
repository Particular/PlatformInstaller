using Caliburn.Micro;

public class InstallingViewModel : Screen,
    IHandle<InstallerOutputEvent>,
    IHandle<InstallProgressEvent>,
    IHandle<DownloadProgressEvent>,
    IHandle<DownloadStartedEvent>,
    IHandle<DownloadCompleteEvent>
{
    public InstallingViewModel()
    {

    }
    public InstallingViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "Installing";
    }

    public string CurrentStatus { get; set; }
    IEventAggregator eventAggregator;
    public int NestedActionPercentComplete { get; set; }
    public bool HasNestedAction { get; set; }
    public string NestedActionDescription { get; set; }
    public bool InstallFailed { get; set; }
    public int InstallProgress { get; set; }
    public int InstallCount { get; set; }
    public bool Downloading { get; set; }

    public void Back()
    {
        eventAggregator.Publish<NavigateHomeCommand>();
    }

    public void Handle(InstallerOutputEvent message)
    {
        if (message.IsError)
        {
            InstallFailed = true;
        }
    }

    public void Handle(InstallProgressEvent message)
    {
        CurrentStatus = message.CurrentStatus;
        InstallProgress = message.InstallProgress;
        InstallCount = message.InstallCount;
    }

    public void Handle(DownloadProgressEvent message)
    {
        NestedActionPercentComplete = message.ProgressPercentage;
        NestedActionDescription = $"{((int) message.BytesReceived).ToBytesString()} of {((int) message.TotalBytes).ToBytesString()}";
    }

    public void Handle(DownloadStartedEvent message)
    {
       Downloading = true;
       HasNestedAction = true;
       NestedActionPercentComplete = 0;
    }

    public void Handle(DownloadCompleteEvent message)
    {
        Downloading = false;
        NestedActionPercentComplete = 0;
        NestedActionDescription = "";
        HasNestedAction = false;
    }
}
