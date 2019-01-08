using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Win32;

public class ServiceControlInstaller : IInstaller
{
    ProcessRunner processRunner;
    ReleaseManager releaseManager;
    Release[] releases;
    IEventAggregator eventAggregator;

    const string RegPath = @"SOFTWARE\ParticularSoftware\ServiceControl";
    const string ManagementRegValue = "ManagementApp";

    public ServiceControlInstaller(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
    {
        this.processRunner = processRunner;
        this.releaseManager = releaseManager;
        this.eventAggregator = eventAggregator;
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
            Text = "ServiceControl documentation",
            Url = "https://docs.particular.net/servicecontrol/"
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
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = $"Downloading {Name}" });

        var release = releases.First();
        var installer = await releaseManager.DownloadRelease(release.Assets.First()).ConfigureAwait(false);
        if (installer == null)
        {
            logError("Failed to download the ServiceControl Installation from https://github.com/Particular/ServiceControl/releases/latest. Please manually download and run the install.");
            return;
        }

        var msiLog = Path.Combine(Logging.LogDirectory,"particular.servicecontrol.installer.log");
        File.Delete(msiLog);

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = $"Executing {Name} installation" });
        var exitCode = await processRunner.RunProcess(installer.FullName,
            $"/quiet /L*V {msiLog.QuoteForCommandline()}",
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
            logError($"Installation of ServiceControl failed with exitcode: {exitCode}");
            if (File.Exists(msiLog))
            {
                logError($"The MSI installation log can be found at {msiLog}");
            }
            else
            {
                logError($"No MSI installation log found. Please check the Application log in the Windows Eventlog");
            }
        }

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
    }

    public static string GetManagementPath()
    {
        if (Environment.Is64BitOperatingSystem)
        {
            var path = ReadRegString(RegistryHive.LocalMachine, RegistryView.Registry64, RegPath, ManagementRegValue);
            if (path != null)
            {
                return path;
            }
        }
        return ReadRegString(RegistryHive.LocalMachine, RegistryView.Registry32, RegPath, ManagementRegValue);
    }

    static string ReadRegString(RegistryHive hive, RegistryView view, string subkey, string valueName)
    {
        using (var key = RegistryKey.OpenBaseKey(hive, view).OpenSubKey(subkey))
        {
            return (string)key?.GetValue(valueName);
        }
    }

    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield return new AfterInstallAction
        {
            Text = "Start ServiceControl Management",
            Description = "Launch this utility to complete the installation or upgrade of the ServiceControl services.",

            Action = () =>
            {
                var value = GetManagementPath();
                if (File.Exists(value))
                {
                    processRunner.RunProcess(value, "", Path.GetDirectoryName(value), s => { }, s => { });
                }
                else
                {
                    eventAggregator.PublishOnUIThread(new FailureEvent{ FailureDescription = "ServiceControl Management Utility not found", FailureText = DetermineDetailsOfManagementUtilityNotFound()});
                }
            }
        };
    }

    string DetermineDetailsOfManagementUtilityNotFound()
    {
        var sb = new StringBuilder();
        sb.AppendLine("ServiceControl Management Utility was not found!");
        sb.AppendLine("Dump of settings related to failure:");
        sb.AppendLine($"ServiceControl Installed : {InstallState != InstallState.NotInstalled} ");
        sb.AppendLine($"ServiceControl Version : {CurrentVersion()} ");
        if (Environment.Is64BitOperatingSystem)
        {
            sb.AppendLine($"64 Bit Management Path : {ReadRegString(RegistryHive.LocalMachine, RegistryView.Registry64, RegPath, ManagementRegValue)}");
        }
        sb.AppendLine($"32 Bit Management Path : {ReadRegString(RegistryHive.LocalMachine, RegistryView.Registry32, RegPath, ManagementRegValue)}");
        return sb.ToString();
    }

    public string Name => "ServiceControl";
    public string Description => "Activity Information - Required for ServiceInsight and ServicePulse";
    public int NestedActionCount => 2;  //Download and Install
    public string ImageName => Name;
    public string Status => this.ExeInstallerStatus();
    public InstallState InstallState { get; private set; }
    public bool SelectedByDefault => InstallState == InstallState.Installed;
    public bool RebootRequired => false;
}