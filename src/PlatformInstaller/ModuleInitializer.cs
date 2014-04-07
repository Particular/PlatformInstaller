public static class ModuleInitializer
{
    public static void Initialize()
    {
        Logging.Initialise();
        ExceptionHandler.Attach();
    }
}