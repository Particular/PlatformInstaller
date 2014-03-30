using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

public class InstallingViewModel : Screen
{
    public InstallingViewModel(ProgressService progressService, PackageDefinitionService packageDefinitionDiscovery, ChocolateyInstaller chocolateyInstaller, IEventAggregator eventAggregator, PackageManager packageManager)
    {
        ProgressService = progressService;

        PackageDefinitionService = packageDefinitionDiscovery;
        this.chocolateyInstaller = chocolateyInstaller;
        this.eventAggregator = eventAggregator;
        this.packageManager = packageManager;
    }

    public string CurrentPackageDescription;
    public ProgressService ProgressService;
    public PackageDefinitionService PackageDefinitionService;
    ChocolateyInstaller chocolateyInstaller;
    IEventAggregator eventAggregator;
    PackageManager packageManager;
    public List<string> ItemsToInstall;
    public bool InstallFailed;
    public double InstallProgress;
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
        var installationDefinitions = PackageDefinitionService.GetPackages().Where(p => ItemsToInstall.Contains(p.Name)).ToList();
        var packageDefinitions = installationDefinitions.SelectMany(installationDefinition => installationDefinition.PackageDefinitions).ToList();
        InstallCount = packageDefinitions.Count();

        if (!chocolateyInstaller.IsInstalled())
        {
            InstallCount++;
            await chocolateyInstaller.InstallChocolatey();
            InstallProgress++;
        }
        foreach (var packageDefinition in packageDefinitions)
        {
            CurrentPackageDescription = packageDefinition.Name;
            await packageManager.Install(packageDefinition.Name, packageDefinition.Parameters);
            InstallProgress++;
        }

        if (ProgressService.Failures.Any())
        {
            InstallFailed = true;
        }
        else
        {
            eventAggregator.Publish<InstallSucceededEvent>();
        }
    }
}