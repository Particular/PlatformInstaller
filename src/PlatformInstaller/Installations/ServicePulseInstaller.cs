﻿using System;
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
    }

    public void Init()
    {
        if (releases == null)
            releases = releaseManager.GetReleasesForProduct(Name);
        InstallState = this.ExeInstallState();
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

    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield break;
    }

    public IEnumerable<DocumentationLink> GetDocumentationLinks()
    {
        yield return new DocumentationLink
        {
            Text = "ServicePulse documentation",
            Url = "https://docs.particular.net/servicepulse/"
        };
    }

    public async Task Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = $"Downloading {Name}" });

        var release = releases.First();

        var installer = await releaseManager.DownloadRelease(release.Assets.Single()).ConfigureAwait(false);
        if (installer == null)
        {
            logError("Failed to download the ServicePulse Installation from https://github.com/Particular/ServicePulse/releases/latest. Please manually download and run the install");
            return;
        }

        var msiLog = Path.Combine(Logging.LogDirectory, "particular.servicepulse.installer.log");
        File.Delete(msiLog);

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = $"Executing {Name} installation" });

        var exitCode = await processRunner.RunProcess(installer.FullName,
            $"/L*V {msiLog.QuoteForCommandline()}",
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
            logError($"Installation of ServicePulse failed with exitcode: {exitCode}");
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

    public string Name => "ServicePulse";
    public string Description => "Production Monitoring";
    public int NestedActionCount => 2; //Download and Install
    public string ImageName => Name;
    public string Status => this.ExeInstallerStatus();
    public bool RebootRequired => false;
    public InstallState InstallState { get; private set; }
    public bool SelectedByDefault => InstallState == InstallState.Installed;
}