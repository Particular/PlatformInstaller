using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

public class ServiceMatrix2012Installer : IInstaller
{

    ProcessRunner processRunner;
    ReleaseManager releaseManager;
    Release[] releases;
    IEventAggregator eventAggregator;

    public ServiceMatrix2012Installer(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
    {
        this.processRunner = processRunner;
        this.releaseManager = releaseManager;
        this.eventAggregator = eventAggregator;
        releases = releaseManager.GetReleasesForProduct("ServiceMatrix");
    }


    public Version CurrentVersion()
    {
        Version version;
        VSIXFind.TryFindInstalledVersion("ServiceMatrix", VisualStudioVersions.VS2012, out version);
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
        var progressEvent = new NestedInstallProgressEvent
        {
            Name = "Run ServiceMatrix for VS2012 Installation"
        };
        eventAggregator.PublishOnUIThread(progressEvent);

        var release = releases.First();
        FileInfo vsixFile;
        try
        {
            vsixFile = await releaseManager.DownloadRelease(release.Assets.Single(x => x.Name.Contains("11.0.vsix"))).ConfigureAwait(false);
        }
        catch
        {
            var error = "Failed to download the ServiceMatrix Installation from https://github.com/Particular/ServiceMatrix/releases/latest";
            logError(error);
            return;
        }

        var toolsPath = Environment.GetEnvironmentVariable("VS110COMNTOOLS");
        if (toolsPath == null)
        {
            logError("Visual Studio 2012 environment variable VS110COMNTOOLS is missing");
            return;
        }
        logOutput(string.Format("VS2012 Tools Path: {0}", toolsPath));

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
            logOutput("Installation Succeeded");
        }
        else
        {
            logError("Installation of ServiceMatrix for VS2012 failed with exitcode: " + exitCode);
            var log = LogFinder.FindVSIXLog(VisualStudioVersions.VS2012);
            if (log != null)
            {
                logError(string.Format("The VSIX installation log can be found at {0}", log));
            }
        }

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
    }

    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield return new AfterInstallAction
        {
            Text = "ServiceMatrix documentation",
            Action = () => Link.OpenUri("http://docs.particular.net/servicematrix/")
        };
    }

    public int NestedActionCount
    {
        get { return 1; }
    }

    public string Name
    {
        get { return "ServiceMatrix for Visual Studio 2012"; }
    }

    public string Status
    {
        get { return this.VsixInstallerStatus(VisualStudioVersions.VS2012); }
    }

    public string ToolTip
    {
        get
        {
            if (VisualStudioDetecter.VS2012Installed)
            {
                return null;
            }
            return "Requires Visual Studio 2012 Professional or higher";
        }
    }

    public bool Installed()
    {
        return CurrentVersion() != null;
    }

    public bool SelectedByDefault
    {
        get { return (VisualStudioDetecter.VS2012Installed & !Installed()); }
    }

    public bool Enabled
    {
        get { return !(!VisualStudioDetecter.VS2012Installed | Installed()); }
    }

}
