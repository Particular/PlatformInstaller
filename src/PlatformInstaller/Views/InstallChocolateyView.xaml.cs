using System.Windows.Input;

public partial class InstallChocolateyView
{
    public InstallChocolateyView()
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