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

    bool legacyInstallMode; //For old vs new SC installer, we can pull this out sometime after SC 1.7

    public ServiceControlInstaller(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
    {
        this.processRunner = processRunner;
        this.releaseManager = releaseManager;
        this.eventAggregator = eventAggregator;
        releases = releaseManager.GetReleasesForProduct("ServiceControl");
        legacyInstallMode = DetermineInstallMode();
    }

    bool DetermineInstallMode()
    {
        return LatestAvailableVersion() < new Version("1.7");
    }

    public IEnumerable<DocumentationLink> GetDocumentationLinks()
    {
        yield return new DocumentationLink
        {
            Text = "ServiceControl documentation",
            Url = "http://docs.particular.net/servicecontrol/"
        };
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
    
    public bool SelectedByDefault => LatestAvailableVersion() != CurrentVersion();

    public bool Enabled => !(LatestAvailableVersion() == CurrentVersion());

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

        var optionalParameters = legacyInstallMode
            ? "PlatformInstaller=true"
            : "";
        
        var exitCode = await processRunner.RunProcess(installer.FullName,
            $"/quiet /L*V {log} {optionalParameters}",
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
        if (!legacyInstallMode)
        {
            yield return new AfterInstallAction
            {
                Text = "Start ServiceControl Management",
                Description = "Launch this utility to complete the installation or upgrade of the ServiceControl services.",

                Action = () =>
                {
                    var value = GetManagementPath();
                    processRunner.RunProcess(value, "", Path.GetDirectoryName(value), s => { }, s => { });
                }
            };
        }
    }

    public int NestedActionCount => 1;

    public string Name => "ServiceControl";

    public string Status => this.ExeInstallerStatus();

    public string ToolTip => "ServiceControl is the monitoring brain in the Particular Service Platform";
}
