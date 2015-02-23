using System.Windows;
// ReSharper disable NotAccessedField.Global
using System;
using System.Collections.Generic;
using Caliburn.Micro;

public class FailedInstallationViewModel : Screen
{
    public FailedInstallationViewModel()
    {
        
    }
    public FailedInstallationViewModel(IEventAggregator eventAggregator, string failureReason, List<string> failures)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "Installation Failed";
        FailureDescription = failureReason;
        FailuresText = string.Join(Environment.NewLine, failures);
    }

    IEventAggregator eventAggregator;

    public string FailureDescription { get; set; }

    public string FailuresText { get; set; }

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }

    public void Home()
    {
        eventAggregator.Publish<NavigateHomeCommand>();
    }

    public void ReportError()
    {

        var confirmSendExceptionView = new ConfirmSendExceptionView
        {
            Owner = ShellView.CurrentInstance
        };

        confirmSendExceptionView.ShowDialog();
        if (confirmSendExceptionView.SendExceptionReport)
        {
            eventAggregator.PublishOnUIThread(new ReportInstallFailedEvent
                                              {
                                                  Failure = FailuresText, 
                                                  FailureDetails = FailureDescription
                                              });    
            MessageBox.Show("Exception Report Sent", "Send Complete", MessageBoxButton.OK);
        }
    }
}