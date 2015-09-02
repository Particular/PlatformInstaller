﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

public class ServiceInsightInstallRunner : IInstallRunner
{
    ProcessRunner processRunner;
    ReleaseManager releaseManager;
    Release[] releases;
    IEventAggregator eventAggregator;

    public ServiceInsightInstallRunner(ProcessRunner processRunner, ReleaseManager releaseManager, IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        this.processRunner = processRunner;
        this.releaseManager = releaseManager;
        releases = releaseManager.GetReleasesForProduct("ServiceInsight");
    }


    public Version CurrentVersion()
    {
        Version version;
        RegistryFind.TryFindInstalledVersion("ServiceInsight", out version);
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
            return !(LatestAvailableVersion() == CurrentVersion());
        }
    }


    public string ToolTip
    {
        get
        {
            return "ServiceInsight is a desktop application with features tailored to developers needs";
        }
    }

    public bool SelectedByDefault
    {
        get
        {
            return LatestAvailableVersion() != CurrentVersion();
        }
    }


    public async Task Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = "Run ServiceInsight Installation" });
        var release = releases.First();
        FileInfo installer;
        try
        {
            installer = await releaseManager.DownloadRelease(release.Assets.Single()).ConfigureAwait(false);
        }
        catch
        {
            logError("Failed to download the ServiceInsight Installation from https://github.com/Particular/ServiceInsight/releases/latest");
            return;
        }

        var log = "particular.serviceinsight.installer.log";
        var fullLogPath = Path.Combine(installer.Directory.FullName, log);
        File.Delete(fullLogPath);

        var exitCode = await processRunner.RunProcess(installer.FullName,
            string.Format("/quiet /L*V {0}", log),
            // ReSharper disable once PossibleNullReferenceException
            installer.Directory.FullName,
            logOutput,
            logError)
            .ConfigureAwait(false);

        if (exitCode != 0)
        {
            logError("Installation of ServiceInsight failed with exitcode: " + exitCode);
            logError("The MSI installation log can be found at {0}" + fullLogPath);
        }
        else
        {
            logOutput("Installation Succeeded");
        }

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

    public string Name { get { return "ServiceInsight"; }}

    public string Status
    {
        get { return this.ExeInstallerStatus(); }
    }

}