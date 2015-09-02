using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

public class ServiceControlInstallRunner : IInstallRunner
{
    ProcessRunner processRunner;
    ReleaseManager releaseManager;
    Release[] releases;
    IEventAggregator eventAggregator;

    public ServiceControlInstallRunner(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
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
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = "Run ServiceControl Installation" });

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
        var fullLogPath = Path.Combine(installer.Directory.FullName,log);
        File.Delete(fullLogPath);

        var exitCode = await processRunner.RunProcess(installer.FullName,
            string.Format("/quiet PlatformInstaller=true /L*V {0}", log),
            installer.Directory.FullName,
            logOutput,
            logError)
            .ConfigureAwait(false);

        if (exitCode != 0)
        {
            logError("Installation of ServiceControl failed with exitcode: "+ exitCode);
            logError("The MSI installation log can be found at "+ fullLogPath);
        }
        else
        {
            logOutput("Installation Succeeded");
        }
        
        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
            
    }

    public bool Installed()
    {
        return CurrentVersion() != null;
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
