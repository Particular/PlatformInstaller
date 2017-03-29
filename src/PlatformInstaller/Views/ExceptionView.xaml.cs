using System;
using System.Windows;
using System.Windows.Input;
using Mindscape.Raygun4Net;


public partial class ExceptionView
{
    string exceptonText;
    Exception exception;

    public ExceptionView()
    {
        InitializeComponent();
    }

    public ExceptionView(Exception exception) : this()
    {
        if (exception == null)
        {
            ExceptionTextBox.Text = "Could not derive exception details";
        }
        else
        {
            this.exception = exception;
            exceptonText = ExceptionTextBox.Text = exception.ToString();
        }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    void ExitClick(object sender, RoutedEventArgs e)
    {
        Environment.Exit(1);
    }

    void CopyClick(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(exceptonText);
    }

    void OpenLogsClick(object sender, RoutedEventArgs e)
    {
        Logging.OpenLogDirectory();
    }

    void SendErrorToRaygunClick(object sender, RoutedEventArgs e)
    {
        var confirmSendExceptionView = new ConfirmSendExceptionView
        {
            Owner = this
        };

        confirmSendExceptionView.ShowDialog();

        if (confirmSendExceptionView.SendExceptionReport)
        {
            var client = new RaygunClient(App.RayGunApiKey);
            client.Send(exception);
            MessageBox.Show("Exception Report Sent", "Send Complete", MessageBoxButton.OK);
        }
    }
}