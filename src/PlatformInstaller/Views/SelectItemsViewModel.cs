using System;
using Autofac;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Windows;
using Anotar.Serilog;

public class SelectItemsViewModel : Screen, IHandle<ResumeInstallCommand>
{
    public SelectItemsViewModel()
    {

    }

    public SelectItemsViewModel(IEnumerable<IInstaller> installers, IEventAggregator eventAggregator, PendingRestartAndResume pendingRestartAndResume, ILifetimeScope lifetimeScope, IWindowManager windowManager)
    {
        DisplayName = "Selected Items";
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
    public Visibility LoadingVisibility { get; set; }
    public List<Item> Items { get; set; }
    public PendingRestartAndResume pendingRestartAndResume { get; set; }
    public string AppVersion { get; set; }

    protected override void OnInitialize()
    {
        LoadingVisibility = Visibility.Visible;
        base.OnInitialize();
        Task.Run(() => { Init(); });
    }

    static string GetImage(string name)
    {
        return ResourceResolver.GetPackUrl($"/Images/{name}.png");
    }

    public void Install()
    {
        var selectedItems = Items.Where(p => p.Selected).OrderBy(p => p.Order).Select(x => x.Name).ToList();
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
        public bool Enabled => CheckBoxVisible == Visibility.Visible;
        public event PropertyChangedEventHandler PropertyChanged;
        public string ToolTip;
        public Visibility CheckBoxVisible;
        public string UninstallText;
        public Visibility UninstallVisible;
        public string Description;
        public int Order;

        public void Uninstall()
        {
            var uninstallCommand = new UninstallProductCommand
            {
                Product = Name
            };
            EventAggregator.PublishOnBackgroundThread(uninstallCommand);
        }
    }

    void Init()
    {
        Parallel.ForEach(installers, installer =>
        {
            try
            {
                installer.Init();
            }
            catch (Exception ex)
            {
                eventAggregator.PublishOnUIThread(new FailureEvent
                {
                    FailureDescription = $"Failed to get release information for {installer.Name}",
                    FailureText = ex.Message
                });
            }
        });

        var i = 0;
        Items = installers
            .OrderBy(x => x.ImageName)
            .Select(x => new Item
            {
                Order = i++,
                Name = x.Name,
                ImageUrl = GetImage(x.ImageName),
                Description = x.Description,
                Status = x.Status,
                EventAggregator = eventAggregator,
                Selected = (x.InstallState != InstallState.Installed) && x.SelectedByDefault,
                CheckBoxVisible = x.InstallState != InstallState.Installed ? Visibility.Visible : Visibility.Collapsed,
                UninstallVisible = x.InstallState != InstallState.NotInstalled ? Visibility.Visible : Visibility.Hidden,
                UninstallText = $"Uninstall {x.Name}"
            }).ToList();

        IsInstallEnabled = Items.Any(pd => pd.Selected);

        Items.BindActionToPropChanged(() =>
        {
            IsInstallEnabled = Items.Any(p => p.Selected);
        }, "Selected");

        LoadingVisibility = Visibility.Collapsed;

        if (pendingRestartAndResume.ResumedFromRestart)
        {
            var pendingInstalls = pendingRestartAndResume.Installs();
            if (pendingInstalls.Count > 0)
            {
                foreach (var item in Items)
                {
                    if (pendingInstalls.Contains(item.Name) && item.CheckBoxVisible == Visibility.Visible)
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }

                eventAggregator.PublishOnUIThread(new ResumeInstallCommand
                {
                    Installs = pendingInstalls
                });
            }
            pendingRestartAndResume.CleanupResume();
        }
    }

    public void Handle(ResumeInstallCommand message)
    {
        using (var beginLifetimeScope = lifetimeScope.BeginLifetimeScope())
        {
            var resumeInstallModel = beginLifetimeScope.Resolve<ResumeInstallViewModel>();
            windowManager.ShowDialog(resumeInstallModel);
            if (!resumeInstallModel.AbortInstallation)
            {
                var runInstallEvent = new RunInstallEvent
                {
                    SelectedItems = message.Installs
                };
                eventAggregator.PublishOnUIThread(runInstallEvent);
            }
        }
    }
}