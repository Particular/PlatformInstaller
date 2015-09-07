using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

public class ServiceMatrix2013Installer : IInstaller
{

    ProcessRunner processRunner;
    ReleaseManager releaseManager;
    Release[] releases;
    IEventAggregator eventAggregator;

    public ServiceMatrix2013Installer(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
    {
        this.processRunner = processRunner;
        this.releaseManager = releaseManager;
        this.eventAggregator = eventAggregator;
        releases = releaseManager.GetReleasesForProduct("ServiceMatrix");
    }
    
    public Version CurrentVersion()
    { 
        Version version;
        VSIXFind.TryFindInstalledVersion("ServiceMatrix", VisualStudioVersions.VS2013, out version);
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
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
        {
            Name = "Run ServiceMatrix for VS2013 Installation"
        });

        var release = releases.First();
        FileInfo vsixFile;
        try
        {
            vsixFile = await releaseManager.DownloadRelease(release.Assets.Single(x => x.Name.Contains("12.0.vsix")));
        }
        catch
        {
            logError("Failed to download the ServiceMatrix Installation from https://github.com/Particular/ServiceMatrix/releases/latest");
            return;
        }

        var toolsPath = Environment.GetEnvironmentVariable("VS120COMNTOOLS");
        if (toolsPath == null)
        {
            logError("Visual Studio 2013 environment variable VS120COMNTOOLS is missing");
            return;
        }
        logOutput(string.Format("VS2013 Tools Path: {0}", toolsPath));

        var toolsInfo = new DirectoryInfo(toolsPath);

        // ReSharper disable once PossibleNullReferenceException
        var vsixInstallerInfo = new FileInfo(Path.Combine(toolsInfo.Parent.FullName, "IDE", "VSIXInstaller.exe"));
        if (!vsixInstallerInfo.Exists)
        {
            logError(string.Format("VSIX Installer not found - {0}", vsixInstallerInfo.FullName));
            return;
        }
        var exitCode = await processRunner.RunProcess(vsixInstallerInfo.FullName,
            string.Format("{0}  /quiet", vsixFile.Name),
            // ReSharper disable once PossibleNullReferenceException
            vsixFile.Directory.FullName,
            logOutput,
            logError)
            .ConfigureAwait(false);

        exitCode = exitCode == 1001 ? 0 : exitCode; //1001 is already installed, treat this as success
        if (exitCode == 0)
        {
            logOutput(string.Format("Installation exitcode: {0}", exitCode));
        }
        else
        {
            logError("Installation of ServiceMatrix for VS2013 failed with exitcode: " + exitCode);
            var log = LogFinder.FindVSIXLog(VisualStudioVersions.VS2013);
            if (log != null)
            {
                logError(string.Format("The VSIX installation log can be found at {0}", log));
            }
        }

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
    }

    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield break;
    }

    public IEnumerable<DocumentationLink> GetDocumentationLinks()
    {
        yield return new DocumentationLink
        {
            Text = "ServiceMatrix for VS2013 documentation",
            Url = "http://docs.particular.net/servicematrix/"
        };
    }

    public int NestedActionCount
    {
        get { return 1; }
    }

    public string Name { get { return "ServiceMatrix for Visual Studio 2013"; }}

    public string Status
    {
        get { return this.VsixInstallerStatus(VisualStudioVersions.VS2013); }
    }

    public bool Installed()
    {
        return CurrentVersion() != null;
    }

    public bool Enabled
    {
        get
        {
            return !(!VisualStudioDetecter.VS2013Installed | Installed());
        }
    }


    public bool SelectedByDefault
    {
        get
        {
            return VisualStudioDetecter.VS2013Installed && !Installed();
        }
    }
    public string ToolTip
    {
        get
        {
            if (VisualStudioDetecter.VS2013Installed)
            {
                return null;
            }
            return "Requires Visual Studio 2013 Professional or higher";
        }
    }

}