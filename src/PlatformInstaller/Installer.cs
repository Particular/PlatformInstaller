// ReSharper disable NotAccessedField.Global
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

public class Installer : IHandle<CancelInstallCommand>, IHandle<NestedInstallCompleteEvent>, IHandle<NestedInstallProgressEvent>
{
    public Installer(InstallationDefinitionService installationDefinitionDiscovery, IEventAggregator eventAggregator, PendingRestartAndResume pendingRestartAndResume)
    {
        installationDefinitionService = installationDefinitionDiscovery;
        this.eventAggregator = eventAggregator;
        this.pendingRestartAndResume = pendingRestartAndResume;
    }

    InstallationDefinitionService installationDefinitionService;
    PendingRestartAndResume pendingRestartAndResume;
    IEventAggregator eventAggregator;
    
    List<string> errors = new List<string>();
    int installProgress;
    int installCount;
    string currentStatus;

    bool InstallFailed
    {
        get { return errors.Any(); }
    }

    bool aborting;
    
    public void Handle(CancelInstallCommand message)
    {
        aborting = true;
        eventAggregator.PublishOnUIThread(new InstallCancelledEvent());
    }

    public async Task Install(List<string> itemsToInstall)
    {
        eventAggregator.PublishOnUIThread(new InstallStartedEvent());
        installProgress = 0;
        var installationDefinitions = installationDefinitionService.GetPackages()
            .Where(p => itemsToInstall.Contains(p.Name))
            .ToList();

        if (pendingRestartAndResume.ResumedFromRestart)
        {
            var checkpoint = pendingRestartAndResume.Checkpoint();
            if (installationDefinitions.Any(p => p.Name.Equals(checkpoint)))
            {
                // Fast Forward to the step after the last successful step
                installationDefinitions = installationDefinitions.SkipWhile(p => !p.Name.Equals(checkpoint)).Skip(1).ToList();
            }
        }
        pendingRestartAndResume.CleanupResume();
        installCount = installationDefinitions.Select(p => p.Installer.NestedActionCount).Sum();

        foreach (var installationDefinition in installationDefinitions)
        {
            if (aborting)
                break;

            PublishProgressEvent();
            var definition = installationDefinition;
            await  Task.Run(() => { definition.Installer.Execute(AddOutput, AddError); } );
            if (InstallFailed)
            {
                eventAggregator.PublishOnUIThread(new InstallFailedEvent
                {
                    Reason = "Failed to install: " + installationDefinition.Name,
                    Failures = errors
                });
                return;
            }
            Thread.Sleep(1000);
            AddOutput(Environment.NewLine);
            eventAggregator.PublishOnUIThread(new CheckPointInstallEvent
            {
                Item = installationDefinition.Name
            });
        }
        if (!InstallFailed)
        {
            eventAggregator.PublishOnUIThread(new InstallSucceededEvent
            {
                InstalledItems = itemsToInstall
            });
        }
    }

    void PublishProgressEvent()
    {
        eventAggregator.PublishOnUIThread(new InstallProgressEvent
                                          {
                                              InstallProgress = installProgress,
                                              CurrentStatus = currentStatus,
                                              InstallCount = installCount,
                                          });
    }

    void AddOutput(string output)
    {
        eventAggregator.PublishOnUIThread(new InstallerOutputEvent
            {
                Text = output
            });
    }

    void AddError(string error)
    {
        eventAggregator.PublishOnUIThread(new InstallerOutputEvent
            {
                IsError = true,
                Text = error
            });
        errors.Add(error);
    }

    public void Handle(NestedInstallCompleteEvent message)
    {
        installProgress++;
        currentStatus = "";
        PublishProgressEvent();

    }

    public void Handle(NestedInstallProgressEvent message)
    {
        currentStatus = message.Name;
        PublishProgressEvent();
    }
}