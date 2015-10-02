using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Win32;

public class PendingRestartAndResume :
    IHandle<RebootRequiredEvent>, 
    IHandle<RunInstallEvent>, 
    IHandle<CheckPointInstallEvent>
{
    public virtual bool ResumedFromRestart { private set;  get; }

    const string runKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    const string platformInstallerKeyPath = @"SOFTWARE\ParticularSoftware\PlatformInstaller";

    public PendingRestartAndResume()
    {
        using (var runKey = Registry.LocalMachine.OpenSubKey(runKeyPath, false))
        {
            ResumedFromRestart = runKey.GetValueNames().Contains("PlatformInstaller");
        }     
    }

    public void Handle(RebootRequiredEvent message)
    {
        AddPendingRestart();
    }

    public void AddPendingRestart()
    {
        var pathValue = $"\"{AssemblyLocation.ExeFilePath}\"";
        using (var runKey = Registry.LocalMachine.OpenSubKey(runKeyPath, true))
        {
            runKey.SetValue("PlatformInstaller", pathValue, RegistryValueKind.String);
        }
    }

    public void RemovePendingRestart()
    {
        using (var runKey = Registry.LocalMachine.OpenSubKey(runKeyPath, true))
        {
            runKey.DeleteValue("PlatformInstaller", false);
        }
    }

    public List<string> Installs()
    {
        using (var installerKey = Registry.LocalMachine.CreateSubKey(platformInstallerKeyPath))
        {
            var items = (string[]) installerKey.GetValue("SelectedInstalls", new string[] {} );
            return items.ToList();
        }
    }

    public string Checkpoint()
    {
        using (var installerKey = Registry.LocalMachine.CreateSubKey(platformInstallerKeyPath))
        {
            return (string) installerKey.GetValue("LastInstallCheckpoint", null);
        }
    }


    public void Handle(RunInstallEvent message)
    {
        using (var installerKey = Registry.LocalMachine.CreateSubKey(platformInstallerKeyPath))
        {
             installerKey.SetValue("SelectedInstalls", message.SelectedItems.ToArray(), RegistryValueKind.MultiString);
        }
    }

    public void Handle(CheckPointInstallEvent message)
    {
        using (var installerKey = Registry.LocalMachine.CreateSubKey(platformInstallerKeyPath))
        {
            installerKey.SetValue("LastInstallCheckpoint", message.Item, RegistryValueKind.String);
        }
    }

    public void CleanupResume()
    {
        using (var installerKey = Registry.LocalMachine.CreateSubKey(platformInstallerKeyPath))
        {
            installerKey.DeleteValue("LastInstallCheckpoint", false);
            installerKey.DeleteValue("SelectedInstalls", false);
        }
    }
}