// ReSharper disable NotAccessedField.Global
using System.Collections.ObjectModel;
using Caliburn.Micro;

public class InstallingViewModel : Screen, IHandle<InstallerOutputEvent>, 
    IHandle<InstallProgressEvent>
{
    public InstallingViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "Installing";
    }

    public string CurrentStatus;
    IEventAggregator eventAggregator;
    public ObservableCollection<InstallerOutputEvent> OutputText = new ObservableCollection<InstallerOutputEvent>();
    public int NestedActionPercentComplete;
    public bool HasNestedAction;
    public string NestedActionDescription;
    public bool InstallFailed;
    public int InstallProgress;
    public int InstallCount;

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