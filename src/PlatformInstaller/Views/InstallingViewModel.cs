using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Caliburn.Micro;

public class InstallingViewModel : Screen
{
    public InstallingViewModel(PackageDefinitionService packageDefinitionDiscovery, ChocolateyInstaller chocolateyInstaller, IEventAggregator eventAggregator, PackageManager packageManager, List<string> itemsToInstall)
    {
        PackageDefinitionService = packageDefinitionDiscovery;
        this.chocolateyInstaller = chocolateyInstaller;
        this.eventAggregator = eventAggregator;
        this.packageManager = packageManager;
        this.itemsToInstall = itemsToInstall;
    }

    public string CurrentStatus;
    public PackageDefinitionService PackageDefinitionService;
    ChocolateyInstaller chocolateyInstaller;
    IEventAggregator eventAggregator;
    PackageManager packageManager;
    List<string> itemsToInstall;
    public List<string> Errors = new List<string>();
    public List<string> Warnings = new List<string>();
    public string OutputText;
    public int NestedActionPercentComplete;
    public bool HasNestedAction;
    public string NestedActionDescription;

    public bool InstallFailed
    {
        get { return Errors.Any(); }
    }

    public int InstallProgress;
    public int InstallCount;

    public void Back()
    {
        eventAggregator.Publish<HomeEvent>();
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        InstallSelected();
    }

    public async Task InstallSelected()
    {
        var packageDefinitions = PackageDefinitionService.GetPackages()
            .Where(p => itemsToInstall.Contains(p.Name))
            .SelectMany(x => x.PackageDefinitions)
            .ToList();
        InstallCount = packageDefinitions.Count();

        if (!chocolateyInstaller.IsInstalled())
        {
            InstallCount++;
            await chocolateyInstaller.InstallChocolatey(OnOutputAction, OnErrorAction);

            if (InstallFailed)
            {
                eventAggregator.Publish(new InstallFailedEvent
                    {
                        Reason = "Failed to install chocolatey",
                        Failures = Errors
                    });
                return;
            }

            ClearNestedAction();
            OutputText += Environment.NewLine;
            InstallProgress++;
        }
        
        foreach (var packageDefinition in packageDefinitions)
        {
            CurrentStatus = packageDefinition.DisplayName ?? packageDefinition.Name;
            await packageManager.Install(packageDefinition.Name, packageDefinition.Parameters, OnOutputAction, OnWarningAction, OnErrorAction, OnProgressAction);


            if (InstallFailed)
            {
                eventAggregator.Publish(new InstallFailedEvent
                    {
                        Reason = "Failed to install package: " + packageDefinition.Name,
                        Failures = Errors
                    });
                return;
            }

            ClearNestedAction();
            OutputText += Environment.NewLine;
            InstallProgress++;
        }



        eventAggregator.Publish<InstallSucceededEvent>();

    }

    void OnProgressAction(ProgressRecord progressRecord)
    {
        if (progressRecord.PercentComplete == -1)
        {
            ClearNestedAction();
            return;
        }

        NestedActionPercentComplete = progressRecord.PercentComplete;
        NestedActionDescription = progressRecord.ToDownloadingString();
    }

    void ClearNestedAction()
    {
        HasNestedAction = false;
        NestedActionPercentComplete = 0;
        NestedActionDescription = "";
    }

    void OnOutputAction(string output)
    {
        OutputText += output + Environment.NewLine;
    }

    void OnErrorAction(string error)
    {
        OutputText += error + Environment.NewLine;
        Errors.Add(error);
    }
    void OnWarningAction(string warning)
    {
        OutputText += warning + Environment.NewLine;
        Warnings.Add(warning);
    }
}