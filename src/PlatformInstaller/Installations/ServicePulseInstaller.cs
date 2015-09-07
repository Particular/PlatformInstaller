using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

public class ServicePulseInstaller : IInstaller
{

    ProcessRunner processRunner;
    ReleaseManager releaseManager;
    Release[] releases;
    IEventAggregator eventAggregator;

    public ServicePulseInstaller(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        this.processRunner = processRunner;
        this.releaseManager = releaseManager;
        releases = releaseManager.GetReleasesForProduct("ServicePulse");
    }

    public Version CurrentVersion()
    {
        Version version;
        RegistryFind.TryFindInstalledVersion("ServicePulse", out version);
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

    public bool Enabled
    {
        get
        {
            return LatestAvailableVersion() != CurrentVersion();
        }
    }


    public string ToolTip
    {
        get
        {
            return "ServicePulse is a web application aimed mainly at administrators";
        }
    }

    public bool SelectedByDefault
    {
        get
        {
            return LatestAvailableVersion() != CurrentVersion();
        }
    }

    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield return new AfterInstallAction
        {
            Text = "Open ServicePulse documentation",
            Action = () => Link.OpenUri("http://docs.particular.net/servicepulse/")
        };
    }

    public int NestedActionCount
    {
        get { return 1; }
    }

    public string Name { get { return "ServicePulse"; } }

    public string Status
    {
        get { return this.ExeInstallerStatus(); }
    }

    public async Task Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = "Run ServicePulse Installation" });
            
        var release = releases.First();
        FileInfo installer;
        try
        {
            installer = await releaseManager.DownloadRelease(release.Assets.Single()).ConfigureAwait(false);
        }
        catch
        {
            logError("Failed to download the ServicePulse Installation from https://github.com/Particular/ServicePulse/releases/latest");
            return;
        }

        var log = "particular.servicepulse.installer.log";
        var fullLogPath = Path.Combine(installer.Directory.FullName, log);
        File.Delete(fullLogPath);

        var exitCode = await processRunner.RunProcess(installer.FullName,
            string.Format("/quiet /L*V {0}", log),
            // ReSharper disable once PossibleNullReferenceException
            installer.Directory.FullName,
            logOutput,
            logError).ConfigureAwait(false);

        if (exitCode == 0)
        {
            logOutput("Installation Succeeded");
        }
        else
        {
            logError("Installation of ServicePulse failed with exitcode: " + exitCode);
            logError("The MSI installation log can be found at "+ fullLogPath);
        }
        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
    }
        
    public bool Installed()
    {
        return CurrentVersion() != null;
    }
    
}
