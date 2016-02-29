using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Autofac;
using Caliburn.Micro;

public static class Runner
{ 
    public static void Run()
    {
        if (!IsAdminChecker.IsAdministrator())
        {
            RelaunchAsElevatedInstance(); 
        }
        RunSingleInstance();
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
            using (Process.Start(processStartInfo)){}
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

    static void RunSingleInstance()
    {
        // Check if this is a second instance
        bool mutexCreated;
        using (new Mutex(true, "particularPlatformInstaller", out mutexCreated))
        {
            if (!mutexCreated)
            {
                MessageBox.Show("An instance of the Platform Installer is already running.", "");
                return;
            }
            InnerRun();
        }
    }

    static void InnerRun()
    {
        Splash.Show();
        Logging.Initialise();
        ExceptionHandler.Attach();
        try
        {
            using (var container = ContainerFactory.BuildContainer())
            {
                container.Resolve<PendingRestartAndResume>().RemovePendingRestart();
                container.Resolve<AutoSubscriber>().Subscribe();
                var appBootstrapper = container.Resolve<AppBootstrapper>();
                appBootstrapper.Initialize();
                container.Resolve<IWindowManager>().ShowDialog(container.Resolve<ShellViewModel>());
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler.HandleException(exception, "Failed at startup");
        }
    }
}