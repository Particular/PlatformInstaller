// ReSharper disable NotAccessedField.Global
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;

public class SelectItemsViewModel : Screen
{
    public SelectItemsViewModel(PackageDefinitionService packageDefinitionDiscovery, IEventAggregator eventAggregator)
    {
        this.packageDefinitionDiscovery = packageDefinitionDiscovery;
        this.eventAggregator = eventAggregator;
    }

    public bool IsInstallEnabled;
    PackageDefinitionService packageDefinitionDiscovery;
    IEventAggregator eventAggregator;
    public List<PackageDefinitionBindable> PackageDefinitions;

    protected override void OnInitialize()
    {
        base.OnInitialize(); 
        PackageDefinitions = packageDefinitionDiscovery
          .GetPackages()
          .OrderBy(p => p.SortOrder)
          .Select(x => new PackageDefinitionBindable
              {
                  ImageUrl = ResourceResolver.GetPackUrl(x.Image),
                  ToolTip = x.ToolTip,
                  Enabled = !x.Disabled,
                  Selected = x.SelectedByDefault,
                  Status = x.Status ?? (x.SelectedByDefault ? "Install" : "Update"),
                  Name = x.Name,
              }).ToList();

        IsInstallEnabled = PackageDefinitions.Any(pd => pd.Selected);

        PackageDefinitions.BindActionToPropChanged(() =>
        {
            IsInstallEnabled = PackageDefinitions.Any(p => p.Selected);
        }, "Selected");
    }

    public void Install()
    {
        var selectedItems = PackageDefinitions.Where(p => p.Selected).Select(x => x.Name).ToList();
        var runInstallEvent = new RunInstallEvent
        {
            SelectedItems = selectedItems
        };
        eventAggregator.Publish(runInstallEvent);
    }

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationEvent>();
    }

    public class PackageDefinitionBindable : INotifyPropertyChanged
    {
        public string Name;
        public string ImageUrl;
        public string Status;
        public bool Selected;
        public bool Enabled;
        public event PropertyChangedEventHandler PropertyChanged;
        public string ToolTip;
    }
}