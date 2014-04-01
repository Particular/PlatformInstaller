using System;
using System.Collections.Generic;
using Caliburn.Micro;

public class FailedInstallationViewModel : Screen
{
    public FailedInstallationViewModel(IEventAggregator eventAggregator, string failureReason,List<string> failures)
    {
        this.eventAggregator = eventAggregator;
        FailureDescription = failureReason;

        FailuresText = string.Join(Environment.NewLine, failures);
    }

    IEventAggregator eventAggregator;

    public string FailureDescription { get; set; }

    public string FailuresText { get; set; }

    public void Close()
    {
        eventAggregator.Publish<CloseApplicationEvent>();
    }

    public void OpenLogDirectory()
    {
        eventAggregator.Publish<OpenLogDirectoryEvent>();
    }
}