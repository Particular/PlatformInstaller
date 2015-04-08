// ReSharper disable NotAccessedField.Global
using System.Collections.ObjectModel;
using Caliburn.Micro;

public class InstallingViewModel : Screen, IHandle<InstallerOutputEvent>, 
    IHandle<InstallProgressEvent>
{
    public InstallingViewModel()
    {
        OutputText = new ObservableCollection<InstallerOutputEvent>();
    }

    public InstallingViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        // ReSharper disable once DoNotCallOverridableMethodsInConstructor
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
        HasNestedAction = message.HasNestedAction;
        NestedActionPercentComplete = message.NestedActionPercentComplete;
        NestedActionDescription = message.NestedActionDescription;
    }
}