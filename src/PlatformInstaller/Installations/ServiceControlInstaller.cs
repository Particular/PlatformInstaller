using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Win32;

public class ServiceControlInstaller : IInstaller
{
    ProcessRunner processRunner;
    ReleaseManager releaseManager;
    Release[] releases;
    IEventAggregator eventAggregator;

    public ServiceControlInstaller(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
    {
        this.processRunner = processRunner;
        this.releaseManager = releaseManager;
        this.eventAggregator = eventAggregator;
        releases = releaseManager.GetReleasesForProduct("ServiceControl");
    }

    public Version CurrentVersion()
    {   
        Version version;
        RegistryFind.TryFindInstalledVersion("ServiceControl", out version);
        return version;
    }

    public Version LatestAvailableVersion()
    {
        Version latest = null;
        if (releases.Any())
        {
            Version.TryParse(releases.First().Tag, out latest);
        }
        return latest;
    }

    public bool SelectedByDefault
    {
        get { return LatestAvailableVersion() != CurrentVersion(); }
    }

    public bool Enabled
    {
        get { return !(LatestAvailableVersion() == CurrentVersion()); }
    }

    public async Task Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
        {
            Name = "Run ServiceControl Installation"
        });

        var release = releases.First();
        FileInfo installer;
        try
        {
            installer = await releaseManager.DownloadRelease(release.Assets.First()).ConfigureAwait(false);
        }
        catch
        {
            logError("Failed to download the ServiceControl Installation from https://github.com/Particular/ServiceControl/releases/latest");
            return;
        }


        var log = "particular.servicecontrol.installer.log";
        var fullLogPath = Path.Combine(installer.Directory.FullName, log);
        File.Delete(fullLogPath);

        var exitCode = await processRunner.RunProcess(installer.FullName,
            string.Format("/quiet /L*V {0}", log),
            installer.Directory.FullName,
            logOutput,
            logError)
            .ConfigureAwait(false);

        if (exitCode == 0)
        {
            logOutput("Installation Succeeded");
        }
        else
        {
            logError("Installation of ServiceControl failed with exitcode: " + exitCode);
            logError("The MSI installation log can be found at " + fullLogPath);
        }

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
    }

    public static string GetManagementPath()
    {
        var name = @"SOFTWARE\ParticularSoftware\ServiceControl";
        if (Environment.Is64BitOperatingSystem)
        {
            var key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(name);
            if (key64 != null)
            {
                return (string) key64.GetValue("ManagementApp");
            }
        }
        var key32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(name);
        return (string) key32.GetValue("ManagementApp");
    }

    public bool Installed()
    {
        return CurrentVersion() != null;
    }

    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield return new AfterInstallAction
        {
            Text = "Open ServiceControl documentation",
            Action = () => Link.OpenUri("http://docs.particular.net/servicecontrol/")
        };
        yield return new AfterInstallAction
        {
            Text = "Open ServiceControl Management",
            Action = () =>
            {
                var value = GetManagementPath();
                processRunner.RunProcess(value, "", Path.GetDirectoryName(value), s => { }, s => { });
            }
        };
    }

    public int NestedActionCount
    {
        get { return 1; }
    }

    public string Name { get { return "ServiceControl"; } }

    public string Status
    {
        get { return this.ExeInstallerStatus(); }
    }

    public string ToolTip
    {
        get
        {
            return "ServiceControl is the monitoring brain in the Particular Service Platform";
        }
    }
}
