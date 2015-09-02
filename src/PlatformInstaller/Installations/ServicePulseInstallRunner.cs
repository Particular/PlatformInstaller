using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

public class ServicePulseInstallRunner : IInstallRunner
{
    const string ProductName = "ServicePulse";

    ProcessRunner processRunner;
    ReleaseManager releaseManager;
    Release[] releases;
    IEventAggregator eventAggregator;

    public ServicePulseInstallRunner(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        this.processRunner = processRunner;
        this.releaseManager = releaseManager;
    }

    public Version CurrentVersion()
    {
        Version version;
        RegistryFind.TryFindInstalledVersion(ProductName, out version);
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

    public bool Disabled
    {
        get
        {
            return LatestAvailableVersion() == CurrentVersion();
        }
    }


    public bool SelectedByDefault
    {
        get
        {
            return LatestAvailableVersion() != CurrentVersion();
        }
    }

    public int NestedActionCount
    {
        get { return 1; }
    }

    public string Name { get { return "ServicePulse"; } }

    public string Status()
    {
        return this.ExeInstallerStatus();
    }

    public async Task Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = string.Format("Run {0} Installation", ProductName)});
            
        var release = releases.First();
        FileInfo installer;
        try
        {
            installer = await releaseManager.DownloadRelease(release.Assets.Single()).ConfigureAwait(false);
        }
        catch
        {
            logError(string.Format("Failed to download the {0} Installation from https://github.com/Particular/{0}/releases/latest", ProductName.ToLower()));
            return;
        }

        var log = string.Format("particular.{0}.installer.log", ProductName.ToLower());
        var fullLogPath = Path.Combine(installer.Directory.FullName, log);
        File.Delete(fullLogPath);

        var exitCode = await processRunner.RunProcess(installer.FullName,
            string.Format("/quiet /L*V {0}", log),
            // ReSharper disable once PossibleNullReferenceException
            installer.Directory.FullName,
            logOutput,
            logError).ConfigureAwait(false);

        if (exitCode != 0)
        {
            logError(string.Format("Installation of {0} failed with exitcode: {1}", ProductName, exitCode));
            logError(string.Format("The MSI installation log can be found at {0}", fullLogPath));
        }
        else
        {
            logOutput("Installation Succeeded");
        }
        InstallationResult = exitCode;
        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
    }
        
    public bool Installed()
    {
        return CurrentVersion() != null;
    }

    public void GetReleaseInfo()
    {
        releases = releaseManager.GetReleasesForProduct(ProductName);
    }

    public bool HasReleaseInfo()
    {
        return (releases != null) && (releases.Length > 0);
    }

    public int InstallationResult { get; private set; }
}
