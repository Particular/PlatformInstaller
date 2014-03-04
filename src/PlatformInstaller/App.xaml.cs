using System.IO;
using System.Windows;
using Serilog;

namespace PlatformInstaller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(Path.Combine(AssemblyLocation.CurrentDirectory,"log.txt"))
                .CreateLogger();
        }
    }

}