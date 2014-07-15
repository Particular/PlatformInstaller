// ReSharper disable NotAccessedField.Global
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Autofac;
using Caliburn.Micro;

public class InstallingViewModel : Screen
{
    public InstallingViewModel(PackageDefinitionService packageDefinitionDiscovery, ChocolateyInstaller chocolateyInstaller, IEventAggregator eventAggregator, PackageManager packageManager, IWindowManager windowManager, PowerShellRunner powerShellRunner, PendingRestartAndResume pendingRestartAndResume, List<string> itemsToInstall, ILifetimeScope lifetimeScope)
    {
        PackageDefinitionService = packageDefinitionDiscovery;
        this.chocolateyInstaller = chocolateyInstaller;
        this.eventAggregator = eventAggregator;
        this.packageManager = packageManager;
        this.windowManager = windowManager;
        this.powerShellRunner = powerShellRunner;
        this.itemsToInstall = itemsToInstall;
        this.lifetimeScope = lifetimeScope;
        this.pendingRestartAndResume = pendingRestartAndResume;
    }

    public string CurrentStatus;
    public PackageDefinitionService PackageDefinitionService;
    public PendingRestartAndResume pendingRestartAndResume;
    ChocolateyInstaller chocolateyInstaller;
    IEventAggregator eventAggregator;
    PackageManager packageManager;
    IWindowManager windowManager;
    PowerShellRunner powerShellRunner;
    List<string> itemsToInstall;
    ILifetimeScope lifetimeScope;
    public List<string> Errors = new List<string>();
    public List<string> Warnings = new List<string>();
    public ObservableCollection<OutputLine> OutputText = new ObservableCollection<OutputLine>();
    public int NestedActionPercentComplete;
    public bool HasNestedAction;
    public string NestedActionDescription;

    public bool InstallFailed
    {
        get { return Errors.Any(); }
    }

    public int InstallProgress;
    public int InstallCount;
    bool aborting;
    bool isInstalling;

    public void Back()
    {
        eventAggregator.Publish<HomeEvent>();
    }

    public override void CanClose(Action<bool> callback)
    {
        if (isInstalling)
        {
            using (var beginLifetimeScope = lifetimeScope.BeginLifetimeScope())
            {
                var confirmModel = beginLifetimeScope.Resolve<ConfirmAbortInstallViewModel>();
                windowManager.ShowDialog(confirmModel);
                if (confirmModel.AbortInstallation)
                {
                    aborting = true;
                    powerShellRunner.Abort();
                    callback(true);
                    eventAggregator.Publish<InstallCancelledEvent>();
                    return;
                }
                callback(false);
            }
        }
        callback(true);
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        InstallSelected();
    }

    public async Task InstallSelected()
    {
        isInstalling = true;
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

        InstallCount = packageDefinitions.Count();
        
        if (!chocolateyInstaller.IsInstalled())
        {
            InstallCount++;
            await chocolateyInstaller.InstallChocolatey(AddOutput, AddError);

            if (InstallFailed)
            {
                isInstalling = false;
                eventAggregator.Publish(new InstallFailedEvent
                    {
                        Reason = "Failed to install chocolatey",
                        Failures = Errors
                    });
                return;
            }

            ClearNestedAction();
            AddOutput(Environment.NewLine);
            InstallProgress++;
        }



        foreach (var packageDefinition in packageDefinitions)
        {
            if (aborting)
            {
                return;
            }
            CurrentStatus = packageDefinition.DisplayName ?? packageDefinition.Name;
            await packageManager.Install(packageDefinition.Name, packageDefinition.Parameters, AddOutput, AddWarning, AddError, OnProgressAction);


            if (InstallFailed)
            {
                isInstalling = false;
                eventAggregator.Publish(new InstallFailedEvent
                    {
                        Reason = "Failed to install package: " + packageDefinition.Name,
                        Failures = Errors
                    });
                return;
            }

            ClearNestedAction();
            AddOutput(Environment.NewLine);
            eventAggregator.Publish(new CheckPointInstallEvent{ Item = packageDefinition.Name});
            InstallProgress++;
        }

        isInstalling = false;
        eventAggregator.Publish(new InstallSucceededEvent { InstalledItems = itemsToInstall });
    }

    void OnProgressAction(ProgressRecord progressRecord)
    {
        if (progressRecord.PercentComplete == -1)
        {
            ClearNestedAction();
            return;
        }

        HasNestedAction = true;
        NestedActionPercentComplete = progressRecord.PercentComplete;
        NestedActionDescription = progressRecord.ToDownloadingString();
    }

    void ClearNestedAction()
    {
        HasNestedAction = false;
        NestedActionPercentComplete = 0;
        NestedActionDescription = "";
    }

    void AddOutput(string output)
    {
        OutputText.Add(new OutputLine
            {
                Text = output
            });
    }

    void AddError(string error)
    {
        OutputText.Add(new OutputLine
            {
                IsError = true,
                Text = error
            });
        Errors.Add(error);
    }

    void AddWarning(string warning)
    {
        OutputText.Add(new OutputLine
            {
                IsWarning = true,
                Text = warning
            });
        Warnings.Add(warning);
    }

    public class OutputLine
    {
        public bool IsError;
        public bool IsWarning;
        public string Text;
    }
}