// ReSharper disable NotAccessedField.Global
using System.Collections.ObjectModel;
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
        OutputText = new ObservableCollection<InstallerOutputEvent>();
    }

    public InstallingViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "Installing";
        OutputText = new ObservableCollection<InstallerOutputEvent>();
    }

    public string CurrentStatus { get; set; }
    IEventAggregator eventAggregator;
    public ObservableCollection<InstallerOutputEvent> OutputText { get; set; }
    public int NestedActionPercentComplete { get; set; }
    public bool HasNestedAction { get; set; }
    public string NestedActionDescription { get; set; }
    public bool InstallFailed { get; set; }
    public int InstallProgress { get; set; }
    public int InstallCount { get; set; }

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
        OutputText.Add(message);
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
        NestedActionDescription = string.Format("{0} of {1}",  ((int)message.BytesReceived).ToBytesString(), ((int)message.TotalBytes).ToBytesString());
    }

    public void Handle(DownloadStartedEvent message)
    {
       OutputText.Add(new InstallerOutputEvent{ Text = string.Format("Downloading {0} to {1}", message.Url, message.FileName)});
       HasNestedAction = true;
       NestedActionPercentComplete = 0;
    }

    public void Handle(DownloadCompleteEvent message)
    {
        NestedActionPercentComplete = 0;
        NestedActionDescription = "";
        HasNestedAction = false;
    }
   
}
