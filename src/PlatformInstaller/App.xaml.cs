using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;

public partial class App
{
    public App()
    {
        InitializeComponent();
    }
    
    public const string RayGunApiKey = "FOsaSSOiJ59Np6IQKHx4Kg==";

    [STAThread]
    public static void Main()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        if (!IsAdminChecker.IsAdministrator())
        {
            RelaunchAsElevatedInstance();
        }
#if (!DEBUG)
        bool mutexCreated;
        using (new Mutex(true, "particularPlatformInstaller", out mutexCreated))
        {
            if (!mutexCreated)
            {
                MessageBox.Show("An instance of the Platform Installer is already running.", "");
                return;
            }
        }
#endif
        var splash = new SplashScreen(typeof(App).Assembly, @"Images\Splash.png");
        splash.Show(true);
        Logging.Initialise();
        ExceptionHandler.Attach();
        var app = new App();
        app.Run();
    }

    static void RelaunchAsElevatedInstance()
    {
        const int CancelledByUser = 1223;
        var processStartInfo = new ProcessStartInfo
        {
            FileName = AssemblyLocation.ExeFileName,
            Verb = "runas"
        };
        try
        {
            using (Process.Start(processStartInfo)) { }
        }
        catch (Win32Exception ex)
        {
            // if user presses no to UAC it will throw an exception
            if (ex.NativeErrorCode != CancelledByUser)
            {
                throw;
            }
        }
        Environment.Exit(0);
    }
}
