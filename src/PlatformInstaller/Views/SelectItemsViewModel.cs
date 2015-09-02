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
    public SelectItemsViewModel(IEnumerable<IInstallRunner> installRunners, IEventAggregator eventAggregator, PendingRestartAndResume pendingRestartAndResume, ILifetimeScope lifetimeScope, IWindowManager windowManager, ReleaseManager releaseManager)
    {
        DisplayName = "Selected Items";
        AppVersion = string.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
        this.installRunners = installRunners.ToList();
        this.eventAggregator = eventAggregator;
        this.pendingRestartAndResume = pendingRestartAndResume;
        this.windowManager = windowManager;
        this.lifetimeScope = lifetimeScope;
    }

    IWindowManager windowManager;
    ILifetimeScope lifetimeScope;

    public bool IsInstallEnabled { get; set; }

    public string AppVersion { get; set; }

    List<IInstallRunner> installRunners;
    IEventAggregator eventAggregator;
    public List<PackageDefinitionBindable> PackageDefinitions { get; set; }
    public PendingRestartAndResume pendingRestartAndResume { get; set; }


   

    protected override void OnInitialize()
    {
        base.OnInitialize(); 
        PackageDefinitions = installRunners
          .Select(x => new PackageDefinitionBindable
              {
                  ImageUrl = GetImage(x.Name),
                  ToolTip = x.ToolTip,
                  Enabled = x.Enabled,
                  Selected = x.SelectedByDefault,
                  Status = x.Status,
                  Name = x.Name,
                  CheckBoxVisible = x.SelectedByDefault,
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

    static string GetImage(string name)
    {
        return ResourceResolver.GetPackUrl("/Images/" + name + ".png");
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
    }
}