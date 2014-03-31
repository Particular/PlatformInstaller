using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Caliburn.Micro;

public class InstallingViewModel : Screen
{
    public InstallingViewModel(PackageDefinitionService packageDefinitionDiscovery, ChocolateyInstaller chocolateyInstaller, IEventAggregator eventAggregator, PackageManager packageManager)
    {
        PackageDefinitionService = packageDefinitionDiscovery;
        this.chocolateyInstaller = chocolateyInstaller;
        this.eventAggregator = eventAggregator;
        this.packageManager = packageManager;
    }

    public string CurrentPackageDescription;
    public PackageDefinitionService PackageDefinitionService;
    ChocolateyInstaller chocolateyInstaller;
    IEventAggregator eventAggregator;
    PackageManager packageManager;
    public List<string> ItemsToInstall;
    public List<string> Errors = new List<string>();
    public List<string> Warnings = new List<string>();
    public string OutputText;

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
            .Where(p => ItemsToInstall.Contains(p.Name))
            .SelectMany(x => x.PackageDefinitions)
            .ToList();
        InstallCount = packageDefinitions.Count();

        if (!chocolateyInstaller.IsInstalled())
        {
            InstallCount++;
            await chocolateyInstaller.InstallChocolatey(OnOutputAction,OnErrorAction);
            InstallProgress++;
        }
        foreach (var packageDefinition in packageDefinitions)
        {
            CurrentPackageDescription = packageDefinition.Name;
            await packageManager.Install(packageDefinition.Name, packageDefinition.Parameters,OnOutputAction,OnWarningAction,OnErrorAction ,OnProgressAction);
            InstallProgress++;
        }

        if (!InstallFailed)
        {
            eventAggregator.Publish<InstallSucceededEvent>();
        }
    }

    void OnProgressAction(ProgressRecord obj)
    {
        

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