namespace PlatformInstaller.Installations
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using NuGet;
    using PlatformInstaller.Releases;
    using PlatformInstaller.Versions;

    public class ServiceControlInstallRunner : IInstallRunner
    {
        const string ProductName = "ServiceControl";
        ProcessRunner processRunner;
        ReleaseManager releaseManager;
        Release[] releases;
        IEventAggregator eventAggregator;

        public ServiceControlInstallRunner(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
        {
            this.processRunner = processRunner;
            this.releaseManager = releaseManager;
            this.eventAggregator = eventAggregator;
        }

        public SemanticVersion CurrentVersion()
        {   
            SemanticVersion version;
            RegistryFind.TryFindInstalledVersion(ProductName, out version);
            return version;
        }

        public SemanticVersion LatestAvailableVersion()
        {
            SemanticVersion latest = null;
            if (releases.Any())
            {
                SemanticVersion.TryParse(releases.First().Tag, out latest);
            }
            return latest;
        }

        public void Execute(Action<string> logOutput, Action<string> logError)
        {
            eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = string.Format("Run {0} Installation", ProductName) });

            var release = releases.First();
            FileInfo[] files;
            try
            {
                files = releaseManager.DownloadRelease(release).ToArray();
            }
            catch
            {
                logError(string.Format("Failed to download the {0} Installation from https://github.com/Particular/{0}/releases/latest", ProductName.ToLower()));
                return;
            }

            
            var installer = files.First(p => p.Extension.Equals(".exe", StringComparison.OrdinalIgnoreCase));
           
            string log = string.Format("particular.{0}.installer.log", ProductName.ToLower());
            var proc = processRunner.RunProcess(installer.FullName,
                string.Format("/quiet PlatformInstaller=true /L*V {0}", log),
                // ReSharper disable once PossibleNullReferenceException
                installer.Directory.FullName,
                logOutput,
                logError);

            Task.WaitAll(proc);
            var procExitCode = proc.Result;
            if (procExitCode != 0)
            {
                logError(string.Format("Installation of {0} failed with exitcode: {1}", ProductName, procExitCode));
                logError(string.Format("The MSI installation log can be found at {0}", Path.Combine(installer.Directory.FullName, log)));
            }
            else
            {
                logOutput("Installation Succeeded");
            }
            InstallationResult = procExitCode;
            Thread.Sleep(1000);

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

        public string Status()
        {
            return this.ExeInstallerStatus();
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
}
