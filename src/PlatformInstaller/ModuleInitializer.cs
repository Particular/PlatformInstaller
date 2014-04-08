using System.ComponentModel;
using System.Windows;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        {
            return;
        }
        Logging.Initialise();
        ExceptionHandler.Attach();
    }
}