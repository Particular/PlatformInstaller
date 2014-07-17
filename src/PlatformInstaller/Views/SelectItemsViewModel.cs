using System;
using Autofac;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using System.Reflection;

public class SelectItemsViewModel : Screen
{
    public SelectItemsViewModel(PackageDefinitionService packageDefinitionDiscovery, IEventAggregator eventAggregator, PendingRestartAndResume pendingRestartAndResume, ILifetimeScope lifetimeScope, IWindowManager windowManager)
    {
        DisplayName = "Selected Items";
        this.packageDefinitionDiscovery = packageDefinitionDiscovery;
        this.eventAggregator = eventAggregator;
        this.pendingRestartAndResume = pendingRestartAndResume;
        this.windowManager = windowManager;
        this.lifetimeScope = lifetimeScope;
    }

    IWindowManager windowManager;
    ILifetimeScope lifetimeScope;
   
    public bool IsInstallEnabled;

    public string AppVersion = String.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);

    PackageDefinitionService packageDefinitionDiscovery;
    IEventAggregator eventAggregator;
    public List<PackageDefinitionBindable> PackageDefinitions;
    public PendingRestartAndResume pendingRestartAndResume;

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

        if (pendingRestartAndResume.ResumedFromRestart)
        {
            using (var beginLifetimeScope = lifetimeScope.BeginLifetimeScope())
            {
                var resumeInstallModel = beginLifetimeScope.Resolve<ResumeInstallViewModel>();
                windowManager.ShowDialog(resumeInstallModel);
                if (resumeInstallModel.AbortInstallation)
                {
                    pendingRestartAndResume.CleanupResume();
                    pendingRestartAndResume.RemovePendingRestart();
                    return;
                }
            }
            var runInstallEvent = new RunInstallEvent
            {
                SelectedItems = pendingRestartAndResume.Installs()
            };
            if (runInstallEvent.SelectedItems.Count > 0)
            {
                eventAggregator.Publish(runInstallEvent);
            }

        }
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