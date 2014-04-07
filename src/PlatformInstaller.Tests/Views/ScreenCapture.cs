using System.Windows;
using System.Windows.Controls;
using ApprovalUtilities.Wpf;
using Caliburn.Micro;

public static class ScreenCapture
{
    public static void TakeScreenShot(this Screen model)
    {
        ShellViewModel.StartModel = model;
        var app = new App();
        app.Activated += (o, args) => WpfUtils.ScreenCapture(app.MainWindow, model.GetType().Name.Replace("ViewModel","") + ".png");
        app.Run();
    }
    public static void TakeScreenShot(Screen model, UserControl view )
    {
        var window = new Window
        {
            Content = view,
            Height = 550,
            Width = 450
        };

        ViewModelBinder.Bind(model, window, null);
        window.Show();
        WpfUtils.ScreenCapture(window, model.GetType().Name.Replace("ViewModel","") + ".png");
    }

}