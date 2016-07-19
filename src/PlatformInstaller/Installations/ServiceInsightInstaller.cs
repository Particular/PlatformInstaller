using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

public class ServiceInsightInstaller : IInstaller
{
    ProcessRunner processRunner;
    ReleaseManager releaseManager;
    Release[] releases;
    IEventAggregator eventAggregator;
    
    public ServiceInsightInstaller(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        this.processRunner = processRunner;
        this.releaseManager = releaseManager;
    }

    public void Init()
    {
        if (releases == null)
            releases = releaseManager.GetReleasesForProduct(Name);
        InstallState = this.ExeInstallState();
    }


    public IEnumerable<DocumentationLink> GetDocumentationLinks()
    {
        yield return new DocumentationLink
        {
            Text = "ServiceInsight documentation",
            Url = "http://docs.particular.net/serviceinsight/"
        };
    }

   
    public Version CurrentVersion()
    {
        Version version;
        RegistryFind.TryFindInstalledVersion(Name, out version);
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


    public async Task Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = "Run ServiceInsight Installation" });
        var release = releases.First();
        var installer = await releaseManager.DownloadRelease(release.Assets.Single()).ConfigureAwait(false);
        if (installer == null)
        {
            logError("Failed to download the ServiceInsight Installation from https://github.com/Particular/ServiceInsight/releases/latest. Please manually download and run the install");
            return;
        }
        
        var log = Path.Combine(Logging.LogDirectory, "particular.serviceinsight.installer.log");
        File.Delete(log);

        var exitCode = await processRunner.RunProcess(installer.FullName,
            $"/quiet /L*V {log}",
            // ReSharper disable once PossibleNullReferenceException
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
            logError($"Installation of ServiceInsight failed with exitcode {exitCode}");
            logError($"The MSI installation log can be found at {log}");
        }

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
    }

    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield break;
    }
    
    public string Name => "ServiceInsight";
    public string Description => "Advanced Debugging";
    public int NestedActionCount => 1;
    public string ImageName => Name;
    public string Status => this.ExeInstallerStatus();
    public InstallState InstallState { get; private set; }
    public bool SelectedByDefault => InstallState == InstallState.Installed;
}