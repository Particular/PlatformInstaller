using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

public class SelectItemsViewModel : Screen
{
    public SelectItemsViewModel(PackageDefinitionService packageDefinitionDiscovery, IEventAggregator eventAggregator)
    {
        PackageDefinitionService = packageDefinitionDiscovery;
        this.eventAggregator = eventAggregator;
        PackageDefinitions = PackageDefinitionService.GetPackages();
        PackageDefinitions.BindActionToPropChanged(() =>
        {
            IsInstallEnabled = PackageDefinitions.Any(p => p.Selected);
        }, "Selected");
    }

    public bool IsInstallEnabled;
    public PackageDefinitionService PackageDefinitionService;
    IEventAggregator eventAggregator;
    public List<PackageDefinition> PackageDefinitions;

    public void Install()
    {
        var selectedItems = PackageDefinitions.Where(p => p.Selected).Select(x => x.Name).ToList();
        var runInstallEvent = new RunInstallEvent
        {
            SelectedItems = selectedItems
        };
        eventAggregator.Publish(runInstallEvent);
    }

    public void Close()
    {
        eventAggregator.Publish<CloseApplicationEvent>();
    }
}