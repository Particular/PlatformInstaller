using System.Windows.Input;

public partial class ResumeInstallView
{
    
    public ResumeInstallView()
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
