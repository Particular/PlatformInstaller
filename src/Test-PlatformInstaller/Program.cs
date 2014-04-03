using System;

class Program
{
    [STAThread]
    public static void Main()
    {
        if (App.DidRelaunchAsAdmin)
        {
            return;
        }
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}