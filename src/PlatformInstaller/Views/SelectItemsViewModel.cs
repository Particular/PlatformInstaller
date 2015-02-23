using System;
using Autofac;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using System.Reflection;

public class SelectItemsViewModel : Screen
{
    public SelectItemsViewModel()
    {
        
    }
    public SelectItemsViewModel(PackageDefinitionService packageDefinitionDiscovery, IEventAggregator eventAggregator, PendingRestartAndResume pendingRestartAndResume, ILifetimeScope lifetimeScope, IWindowManager windowManager)
    {
        DisplayName = "Selected Items";
        AppVersion = String.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
        this.packageDefinitionDiscovery = packageDefinitionDiscovery;
        this.eventAggregator = eventAggregator;
        this.pendingRestartAndResume = pendingRestartAndResume;
        this.windowManager = windowManager;
        this.lifetimeScope = lifetimeScope;
    }

    IWindowManager windowManager;
    ILifetimeScope lifetimeScope;

    public bool IsInstallEnabled { get; set; }

    public string AppVersion { get; set; }

    PackageDefinitionService packageDefinitionDiscovery;
    IEventAggregator eventAggregator;
    public List<PackageDefinitionBindable> PackageDefinitions { get; set; }
    public PendingRestartAndResume pendingRestartAndResume { get; set; }

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
                eventAggregator.PublishOnUIThread(runInstallEvent);
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
        eventAggregator.PublishOnUIThread(runInstallEvent);
    }

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }

    public class PackageDefinitionBindable : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; }
        public bool Selected { get; set; }
        public bool Enabled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public string ToolTip { get; set; }
    }
}