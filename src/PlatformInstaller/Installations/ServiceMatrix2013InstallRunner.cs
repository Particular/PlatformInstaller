using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

public class ServiceMatrix2013InstallRunner : IInstallRunner
{
    const string ProductName = "ServiceMatrix";

    ProcessRunner processRunner;
    ReleaseManager releaseManager;
    Release[] releases;
    IEventAggregator eventAggregator;

    public ServiceMatrix2013InstallRunner(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
    {
        this.processRunner = processRunner;
        this.releaseManager = releaseManager;
        this.eventAggregator = eventAggregator;
    }

    public string InstallableVersion
    {
        get { return releases.First().Tag; }
    }
        
    public Version CurrentVersion()
    { 
        Version version;
        VSIXFind.TryFindInstalledVersion(ProductName, VisualStudioVersions.VS2013, out version);
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

    public void Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = string.Format("Run {0} for VS2013 Installation", ProductName)});

        var release = releases.First();
        FileInfo vsixFile;
        try
        {
            vsixFile = releaseManager.DownloadRelease(release.Assets.Single(x => x.Name.Contains("12.0.vsix")));
        }
        catch
        {
            logError(string.Format("Failed to download the {0} Installation from https://github.com/Particular/{0}/releases/latest", ProductName.ToLower()));
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
            var process = processRunner.RunProcess(vsixInstallerInfo.FullName,
            string.Format("{0}  /quiet", vsixFile.Name),
            // ReSharper disable once PossibleNullReferenceException
            vsixFile.Directory.FullName, 
            logOutput, 
            logError);
           
        Task.WaitAll(process);
        var exitCode = process.Result == 1001 ? 0 : process.Result; //1001 is already installed, treat this as success
        if (exitCode != 0)
        {
            logError(string.Format("Installation of {0} for VS2013 failed with exitcode: {1}", ProductName, exitCode));
            var log = LogFinder.FindVSIXLog(VisualStudioVersions.VS2013);
            if (log != null)
            {
                logError(string.Format("The VSIX installation log can be found at {0}", log));
            }
        }
        else
        {
            logOutput(string.Format("Installation exitcode: {0}", process.Result));
        }
        Thread.Sleep(1000);
        InstallationResult = exitCode;  

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
    }

    public int NestedActionCount
    {
        get { return 1; }
    }

    public string Status()
    {
        return this.VsixInstallerStatus(VisualStudioVersions.VS2013);
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