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

    public bool Enabled => LatestAvailableVersion() != CurrentVersion();

    public string ToolTip => "ServicePulse is a web application aimed mainly at administrators";

    public bool SelectedByDefault => LatestAvailableVersion() != CurrentVersion();

    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield break;
    }

    public IEnumerable<DocumentationLink> GetDocumentationLinks()
    {
        yield return new DocumentationLink
        {
            Text = "ServiceInsight documentation",
            Url = "http://docs.particular.net/serviceinsight/"
        };
    }

    public int NestedActionCount => 1;

    public string Name => "ServicePulse";

    public string Status => this.ExeInstallerStatus();

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
            $"/quiet /L*V {log}",
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
