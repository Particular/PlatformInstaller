using System.Diagnostics;
using Serilog;

public partial class App
{
    static App()
    {
        if (!IsAdminChecker.IsAdministrator())
        {
            Process.Start(new ProcessStartInfo
                {
                    FileName = AssemblyLocation.ExeFileName,
                    Verb = "runas"
                });
            return;
        }
        Logging.Initialise();
        ExceptionHandler.Attach();
        Log.Information(string.Format("Starting PlatformInstaller v{0}", VersionFinder.GetVersion()));
    }

    public App()
    {
        ExceptionHandler.Attach(this);
        new AppBootstrapper().Start();
    }

}