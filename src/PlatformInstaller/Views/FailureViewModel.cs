using System.Windows;
using Caliburn.Micro;

public class FailureViewModel : Screen
{
    IEventAggregator eventAggregator;

    public string FailureDescription { get; set; }

    public string FailureText { get; set; }

    public FailureViewModel()
    {

    }

    public FailureViewModel(IEventAggregator eventAggregator, string FailureDescription, string FailureText)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "No release information";
        this.FailureDescription = FailureDescription;
        this.FailureText = FailureText;
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
            eventAggregator.PublishOnCurrentThread(new ReportInstallFailedEvent
            {
                Failure = FailureText,
                FailureDetails = FailureDescription
            });
            MessageBox.Show("Exception Report Sent", "Send Complete", MessageBoxButton.OK);
        }
    }

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }
}
