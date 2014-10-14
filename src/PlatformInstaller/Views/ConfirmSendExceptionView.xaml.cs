using System.Windows;
using System.Windows.Input;


public partial class ConfirmSendExceptionView
{
    public ConfirmSendExceptionView()
    {
        InitializeComponent();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    public bool SendExceptionReport;

    void ConfirmSendClick(object sender, RoutedEventArgs e)
    {
        SendExceptionReport = true;
        Close();
    }

    void CancelClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

}