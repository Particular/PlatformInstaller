using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;

public class SelectItemsViewModel : Screen
{
    public SelectItemsViewModel(PackageDefinitionService packageDefinitionDiscovery, IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        PackageDefinitions = packageDefinitionDiscovery
            .GetPackages()
            .Select(x=>
            {
                return new PackageDefinitionBindable
                {
                    ImageUrl = "pack://application:,,,/PlatformInstaller;component" + x.Image,
                    ToolTip = x.ToolTip,
                    Enabled = !x.Disabled,
                    Status = x.Status ?? (x.SelectedByDefault ? "Install" : "Update"),
                    Name = x.Name,
                };
            }).ToList();

        IsInstallEnabled = PackageDefinitions.Any(pd => pd.Selected);

        PackageDefinitions.BindActionToPropChanged(() =>
        {
            IsInstallEnabled = PackageDefinitions.Any(p => p.Selected);
        }, "Selected");
    }
    
    public bool IsInstallEnabled;
    IEventAggregator eventAggregator;
    public List<PackageDefinitionBindable> PackageDefinitions;

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