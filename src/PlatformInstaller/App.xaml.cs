using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

public partial class App
{
    static Mutex instanceLockMutex;
    static App()
    {
        if (!IsAdminChecker.IsAdministrator())
        {
            var processStartInfo = new ProcessStartInfo
                {
                    FileName = AssemblyLocation.ExeFileName,
                    Verb = "runas"
                };
            using (Process.Start(processStartInfo))
            {
            }
            Environment.Exit(0);
        }    
        
        // Check if this is a second instance
        bool mutexCreated;
        instanceLockMutex = new Mutex(true, "particularPlatformInstaller", out mutexCreated);
        if (!mutexCreated)
        {
            MessageBox.Show("An instance of the Platform Installer is already running.", "");
            Current.Shutdown();
        }

    }

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