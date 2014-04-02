using System;
using System.Collections.Generic;
using Caliburn.Micro;

public class FailedInstallationViewModel : Screen
{
    public FailedInstallationViewModel(IEventAggregator eventAggregator, string failureReason, List<string> failures)
    {
        this.eventAggregator = eventAggregator;
        FailureDescription = failureReason;
        FailuresText = string.Join(Environment.NewLine, failures);
    }

    IEventAggregator eventAggregator;

    public string FailureDescription;

    public string FailuresText;

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationEvent>();
    }

    public void Home()
    {
        eventAggregator.Publish<HomeEvent>();
    }

}