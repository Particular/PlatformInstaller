using ApprovalUtilities.Wpf;
using Caliburn.Micro;

public static class ScreenCapture
{
    public static void TakeScreenShot(this Screen model)
    {
        ShellViewModel.StartModel = model;
        var app = new App();
        app.Activated += (o, args) => WpfUtils.ScreenCapture(app.MainWindow, model.GetType().Name + ".png");
        app.InitializeComponent();
        app.Run();
    }

}