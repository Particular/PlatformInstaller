using Caliburn.Micro;
using Microsoft.Win32;

public class PendingRestart :
    IHandle<RebootNeeded>
{
    public void AddPendingRestart()
    {
        var pathValue = string.Format("\"{0}\"", AssemblyLocation.ExeFilePath);
        using (var myKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
        {
            myKey.SetValue("PlatformInstaller", pathValue, RegistryValueKind.String);
        }
    }

    public void RemovePendingRestart()
    {
        using (var myKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
        {
            myKey.DeleteValue("PlatformInstaller", false);
        }
    }

    public void Handle(RebootNeeded message)
    {
        AddPendingRestart();
    }
}