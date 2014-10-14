using System.Windows;
using Mindscape.Raygun4Net;
// ReSharper disable NotAccessedField.Global
using System;
using System.Collections.Generic;
using Caliburn.Micro;

public class FailedInstallationViewModel : Screen
{
    public FailedInstallationViewModel(IEventAggregator eventAggregator, string failureReason, List<string> failures)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "Installation Failed";
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

    public void ReportError()
    {

        var confirmSendExceptionView = new ConfirmSendExceptionView()
        {
            Owner = ShellView.CurrentInstance
        };

        confirmSendExceptionView.ShowDialog();
        if (confirmSendExceptionView.SendExceptionReport)
        {
            eventAggregator.Publish(new ReportInstallFailedEvent { Failure = FailuresText, FailureDetails = FailureDescription });    
            MessageBox.Show("Exception Report Sent", "Send Complete", MessageBoxButton.OK);
        }
    }
}