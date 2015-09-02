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
    public SelectItemsViewModel(InstallationDefinitionService installationDefinitionDiscovery, IEventAggregator eventAggregator, PendingRestartAndResume pendingRestartAndResume, ILifetimeScope lifetimeScope, IWindowManager windowManager, ReleaseManager releaseManager)
    {
        DisplayName = "Selected Items";
        AppVersion = string.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
        this.installationDefinitionDiscovery = installationDefinitionDiscovery;
        this.eventAggregator = eventAggregator;
        this.pendingRestartAndResume = pendingRestartAndResume;
        this.windowManager = windowManager;
        this.lifetimeScope = lifetimeScope;
        this.releaseManager = releaseManager;
    }

    IWindowManager windowManager;
    ILifetimeScope lifetimeScope;
    ReleaseManager releaseManager;


    public bool IsInstallEnabled { get; set; }

    public string AppVersion { get; set; }

    InstallationDefinitionService installationDefinitionDiscovery;
    IEventAggregator eventAggregator;
    public List<PackageDefinitionBindable> PackageDefinitions { get; set; }
    public PendingRestartAndResume pendingRestartAndResume { get; set; }

    protected override void OnInitialize()
    {
        base.OnInitialize(); 
        PackageDefinitions = installationDefinitionDiscovery
          .GetPackages()
          .Select(x => new PackageDefinitionBindable
              {
                  ImageUrl = GetImage(x),
                  ToolTip = x.Installer.ToolTip,
                  Enabled = !x.Disabled,
                  Selected = x.Installer.SelectedByDefault,
                  Status = x.FeedOK ? x.Installer.Status : "No Product Feed",
                  Name = x.Installer.Name,
                  CheckBoxVisible = ShowCheckBox(x),
                  FeedOK = x.FeedOK
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
        
        if (releaseManager.FailedFeeds.Count > 0)
        {
            FeedErrors = true;
        }
    }

    static string GetImage(InstallationDefinition x)
    {
        return ResourceResolver.GetPackUrl("/Images/"+ x.Installer.Name+".png");
    }

    bool ShowCheckBox(InstallationDefinition definition)
    {
        return definition.FeedOK && 
            definition.NoErrors && 
            definition.Installer.SelectedByDefault;
    }

    public bool FeedErrors { get; set; }
    
    public void Install()
    {
        var selectedItems = PackageDefinitions.Where(p => p.Selected).Select(x => x.Name).ToList();
        var runInstallEvent = new RunInstallEvent
        {
            SelectedItems = selectedItems
        };
        eventAggregator.PublishOnUIThread(runInstallEvent);
    }

    public void OpenLogs()
    {
        Logging.OpenLogDirectory();
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
        public string InstalledVersion { get; set; }
        public bool Selected { get; set; }
        public bool Enabled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public string ToolTip { get; set; }
        public bool CheckBoxVisible { get; set; }
        public bool FeedOK { get; set; }
    }
}