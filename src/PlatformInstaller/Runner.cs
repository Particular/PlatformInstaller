using System;
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
        using (var container = ContainerFactory.BuildContainer())
        {
            container.Resolve<ChocolateyInstaller>().PatchRunNuget();
            container.Resolve<PendingRestart>().RemovePendingRestart();
            container.Resolve<AutoSubscriber>().SubScribe();
            var appBootstrapper = container.Resolve<AppBootstrapper>();
            appBootstrapper.Start();
            container.Resolve<IWindowManager>().ShowDialog(container.Resolve<ShellViewModel>());
        }
    }
}