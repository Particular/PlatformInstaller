using System.Windows.Input;

public partial class ShellView
{
    public static ShellView CurrentInstance;
    public ShellView()
    {
        CurrentInstance = this;
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
