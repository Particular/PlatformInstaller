public partial class App
{
    public App()
    {
        Logging.Initialise();
        ExceptionHandler.Attach();
    }
}
