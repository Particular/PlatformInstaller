using System.Windows.Input;

public partial class ConfirmAbortInstallView
{
    
    public ConfirmAbortInstallView()
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

    
}
