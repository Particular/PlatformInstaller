using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

public partial class ShellView
{
    public static ShellView CurrentInstance;
    public ShellView()
    {
        CurrentInstance = this;
        InitializeComponent();
        
    }

    public void HideMe()
    {
        var fromVisual = (HwndSource) PresentationSource.FromVisual(this);
        if (fromVisual == null)
        {
            return;
        }
        var hwnd = fromVisual.Handle;
        UnsafeNativeMethods.ShowWindow(hwnd, ShowWindowCommands.Minimize);
    }

    public void ShowMe()
    {
        var fromVisual = (HwndSource) PresentationSource.FromVisual(this);
        if (fromVisual == null)
        {
            return;
        }
        var hwnd = fromVisual.Handle;
        UnsafeNativeMethods.ShowWindow(hwnd, WindowState == WindowState.Normal ? ShowWindowCommands.Maximize : ShowWindowCommands.Normal);
    }
    
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    void OpenLogDirectory(object sender, RoutedEventArgs routedEventArgs)
    {
        Logging.OpenLogDirectory();
    }
}
