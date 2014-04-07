using System.Diagnostics;
using System.Threading;
using System.Windows;
using Serilog;

public partial class App
{
    static Mutex instanceLockMutex;
    static App()
    {
        if (!IsAdminChecker.IsAdministrator())
        {
            Process.Start(new ProcessStartInfo
                {
                    FileName = AssemblyLocation.ExeFileName,
                    Verb = "runas"
                });
            DidRelaunchAsAdmin = true;
            return;
        }    
        
        // Check if this is a second instance
        bool mutexCreated;
        instanceLockMutex = new Mutex(true, "particularPlatformInstaller", out mutexCreated);
        if (!mutexCreated)
        {
            MessageBox.Show("An instance of the Platform Installer is already running.", "");
            Current.Shutdown();
        }

        Logging.Initialise();
        ExceptionHandler.Attach();
        Log.Information(string.Format("Starting PlatformInstaller v{0}", VersionFinder.GetVersion()));
    }

    public static bool DidRelaunchAsAdmin;

    public App()
    {
        ExceptionHandler.Attach(this);
        new AppBootstrapper().Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (instanceLockMutex != null)
        {
            instanceLockMutex.Dispose();
        }
    }
}