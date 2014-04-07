using System;
using System.Windows;
using System.Windows.Input;

public partial class ExceptionView
{
    string exceptonText;

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
            exceptonText = ExceptionTextBox.Text = exception.ToFriendlyString();
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

}