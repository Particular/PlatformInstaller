using System;
using Autofac;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Caliburn.Micro;
using System.Reflection;
using System.Windows;
using Anotar.Serilog;

public class SelectItemsViewModel : Screen
{
    public SelectItemsViewModel()
    {
        
    }
    public SelectItemsViewModel(IEnumerable<IInstaller> installers, IEventAggregator eventAggregator, PendingRestartAndResume pendingRestartAndResume, ILifetimeScope lifetimeScope, IWindowManager windowManager)
    {
        DisplayName = "Selected Items";
        AppVersion = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";
        this.installers = installers.ToList();
        this.eventAggregator = eventAggregator;
        this.pendingRestartAndResume = pendingRestartAndResume;
        this.windowManager = windowManager;
        this.lifetimeScope = lifetimeScope;
    }

    IWindowManager windowManager;
    ILifetimeScope lifetimeScope;
    List<IInstaller> installers;
    IEventAggregator eventAggregator;

    public bool IsInstallEnabled { get; set; }
    public List<Item> Items { get; set; }
    public PendingRestartAndResume pendingRestartAndResume { get; set; }
    public string AppVersion { get; set; }
    
    protected override void OnInitialize()
    {
        base.OnInitialize();
       
        foreach (var installer in  installers)
        {
            try
            {
                installer.Init();
            }
            catch(Exception ex)
            {
                eventAggregator.PublishOnUIThread(
                    new FailureEvent
                    {
                        FailureDescription = $"Failed to get release information for {installer.Name}",
                        FailureText = ex.Message
                    });
                return;
            }
        }

        Items = installers
            .Select(x => new Item
            {
                EventAggregator = eventAggregator,
                ImageUrl = GetImage(x.Name),
                ToolTip = x.ToolTip,
                Enabled = x.Enabled,
                Selected = x.SelectedByDefault,
                Status = x.Status,
                Name = x.Name,
                CheckBoxVisible = x.SelectedByDefault ? Visibility.Visible : Visibility.Collapsed,
                UninstallVisible = x.Installed() ? Visibility.Visible : Visibility.Hidden,
                UninstallText = "Uninstall " + x.Name
            }).ToList();


        IsInstallEnabled = Items.Any(pd => pd.Selected);

        Items.BindActionToPropChanged(() =>
        {
            IsInstallEnabled = Items.Any(p => p.Selected);
        }, "Selected");

        if (!pendingRestartAndResume.ResumedFromRestart)
        {
            return;
        }
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
        var pendingInstalls = pendingRestartAndResume.Installs();
        if (pendingInstalls.Count > 0)
        {
            var runInstallEvent = new RunInstallEvent
            {
                SelectedItems = pendingInstalls
            };
            eventAggregator.PublishOnUIThread(runInstallEvent);
        }
    }

    static string GetImage(string name)
    {
        return ResourceResolver.GetPackUrl("/Images/" + name + ".png");
    }
    
    public void Install()
    {
        var selectedItems = Items.Where(p => p.Selected).Select(x => x.Name).ToList();
        var runInstallEvent = new RunInstallEvent
        {
            SelectedItems = selectedItems
        };
        eventAggregator.PublishOnUIThread(runInstallEvent);
    }

    public void Uninstall(string product)
    {
        LogTo.Debug($"Uninstall {product}");
    }


    public void OpenLogs()
    {
        Logging.OpenLogDirectory();
    }
    
    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }
    
    // Suppressing NotAccessedField.Global 
    // Fields are used via Caliburn.Micro 
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    public class Item : INotifyPropertyChanged
    {
        public IEventAggregator EventAggregator;
        public string Name;
        public string ImageUrl;
        public string Status;
        public string InstalledVersion;
        public bool Selected;
        public bool Enabled;
        public event PropertyChangedEventHandler PropertyChanged;
        public string ToolTip;
        public Visibility CheckBoxVisible;
        public string UninstallText;
        public Visibility UninstallVisible;
        
        public void Uninstall()
        {
            var uninstallCommand = new UninstallProductCommand
            {
                Product = Name
            };
            EventAggregator.PublishOnBackgroundThread(uninstallCommand);
        }
    }
}