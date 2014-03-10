using System.IO;
using Serilog;

namespace PlatformInstaller
{
    public partial class App
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