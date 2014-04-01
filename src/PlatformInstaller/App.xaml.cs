using Serilog;

public partial class App
{
    public App()
    {
        Logging.Initialise();
        ExceptionHandler.Attach();

        Log.Information(string.Format("Starting PlatformInstaller v{0}", PlatformInstaller.Version)); 
        
        new AppBootstrapper().Start();
    }
}