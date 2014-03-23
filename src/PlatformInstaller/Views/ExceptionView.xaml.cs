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
    public ExceptionView(Exception exception):this()
    {
        if (exception != null)
        {
            exceptonText = ExceptionTextBox.Text =  exception.ToFriendlyString();
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

    void CloseClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown(1);
    }

    void CopyClick(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(exceptonText);
    }

    void IgnoreClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
