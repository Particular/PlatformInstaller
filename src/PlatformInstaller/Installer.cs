// ReSharper disable NotAccessedField.Global
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

public class Installer : IHandle<CancelInstallCommand>
{
    public Installer(PackageDefinitionService packageDefinitionDiscovery, ChocolateyInstaller chocolateyInstaller, IEventAggregator eventAggregator, PackageManager packageManager, PowerShellRunner powerShellRunner, PendingRestartAndResume pendingRestartAndResume)
    {
        PackageDefinitionService = packageDefinitionDiscovery;
        this.chocolateyInstaller = chocolateyInstaller;
        this.eventAggregator = eventAggregator;
        this.packageManager = packageManager;
        this.powerShellRunner = powerShellRunner;
        this.pendingRestartAndResume = pendingRestartAndResume;
    }

    string currentStatus;
    PackageDefinitionService PackageDefinitionService;
    PendingRestartAndResume pendingRestartAndResume;
    ChocolateyInstaller chocolateyInstaller;
    IEventAggregator eventAggregator;
    PackageManager packageManager;
    PowerShellRunner powerShellRunner;
    List<string> errors = new List<string>();
    int NestedActionPercentComplete;
    bool HasNestedAction;
    string NestedActionDescription;
    int installProgress;
    int installCount;

    bool InstallFailed
    {
        get { return errors.Any(); }
    }

    bool aborting;
    
    public void Handle(CancelInstallCommand message)
    {
        aborting = true;
        powerShellRunner.Abort();
        eventAggregator.PublishOnUIThread(new InstallCancelledEvent());
    }

    public async Task Install(List<string> itemsToInstall)
    {
        eventAggregator.PublishOnUIThread(new InstallStartedEvent());
        installProgress = 0;
        var packageDefinitions = PackageDefinitionService.GetPackages()
            .Where(p => itemsToInstall.Contains(p.Name))
            .SelectMany(x => x.PackageDefinitions)
            .ToList();

        if (pendingRestartAndResume.ResumedFromRestart)
        {
            var checkpoint = pendingRestartAndResume.Checkpoint();
            if (packageDefinitions.Any(p => p.Name.Equals(checkpoint)))
            {
                // Fast Forward to the step after the last successful step
                packageDefinitions = packageDefinitions.SkipWhile(p => !p.Name.Equals(checkpoint)).Skip(1).ToList();
            }
        }
        pendingRestartAndResume.CleanupResume();

        installCount = packageDefinitions.Count();

        if (!chocolateyInstaller.IsInstalled() || await chocolateyInstaller.ChocolateyUpgradeRequired())
        {
            installCount++;
            PublishProgressEvent();
            await chocolateyInstaller.InstallChocolatey(AddOutput, AddError);

            if (InstallFailed)
            {
                eventAggregator.PublishOnUIThread(new InstallFailedEvent
                                                  {
                                                      Reason = "Failed to install chocolatey",
                                                      Failures = errors
                                                  });
                return;
            }

            ClearNestedAction();
            AddOutput(Environment.NewLine);
            installProgress++;
            PublishProgressEvent();
        }

        foreach (var packageDefinition in packageDefinitions)
        {
            if (aborting)
            {
                return;
            }
            currentStatus = packageDefinition.DisplayName ?? packageDefinition.Name;
            

            //await packageManager.Install(packageDefinition.Name, packageDefinition.Parameters, AddOutput, AddWarning, AddError, OnProgressAction);
            await packageManager.Install(packageDefinition.Name, packageDefinition.Parameters, AddOutput, AddError);
            if (InstallFailed)
            {
                eventAggregator.PublishOnUIThread(new InstallFailedEvent
                    {
                        Reason = "Failed to install package: " + packageDefinition.Name,
                        Failures = errors
                    });
                return;
            }

            ClearNestedAction();
            AddOutput(Environment.NewLine);
            eventAggregator.PublishOnUIThread(new CheckPointInstallEvent{ Item = packageDefinition.Name});
            installProgress++;
            PublishProgressEvent();
        }

        eventAggregator.PublishOnUIThread(new InstallSucceededEvent
                                          {
                                              InstalledItems = itemsToInstall
                                          });
    }

    void PublishProgressEvent()
    {
        eventAggregator.PublishOnUIThread(new InstallProgressEvent
                                          {
                                              InstallProgress = installProgress,
                                              CurrentStatus = currentStatus,
                                              InstallCount = installCount,
                                              HasNestedAction = HasNestedAction,
                                              NestedActionPercentComplete = NestedActionPercentComplete,
                                              NestedActionDescription = NestedActionDescription,
                                          });
    }

    void ClearNestedAction()
    {
        HasNestedAction = false;
        NestedActionPercentComplete = 0;
        NestedActionDescription = "";
        PublishProgressEvent();
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
}