using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;

public class SuccessViewModel : Screen
{
    public SuccessViewModel()
    {
        
    }
    public SuccessViewModel(IEventAggregator eventAggregator, IEnumerable<IInstaller> installers, List<string> installedItemNames)
    {
        DisplayName = "Success";
        this.eventAggregator = eventAggregator;
        this.installers = installers;
        this.installedItemNames = installedItemNames;
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        ActionItems = GetActionItems().ToList();
        FurtherActionsVisible = ActionItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        LinkItems = GetLinkItems().ToList();
    }

    public Visibility FurtherActionsVisible { get; set; }

    public List<LinkItem> LinkItems { get; set; }

    IEnumerable<IInstaller> GetInstalled()
    {
        return installedItemNames.Select(name => installers.Single(x => x.Name == name));
    }

    IEnumerable<ActionItem> GetActionItems()
    {
        return from installer in GetInstalled()
            from action in installer.GetAfterInstallActions()
            select new ActionItem
            {
                Command = new SimpleCommand(action.Action),
                Text = action.Text,
                Description = action.Description,
            };
    }

    IEnumerable<LinkItem> GetLinkItems()
    {
        return from installer in GetInstalled()
            from action in installer.GetDocumentationLinks()
               select new LinkItem
            {
                Text = action.Text,
                Uri = action.Url,
            };
    }

    public List<ActionItem> ActionItems { get; set; }

    IEventAggregator eventAggregator;
    IEnumerable<IInstaller> installers;
    List<string> installedItemNames;

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }

    public void Home()
    {
        eventAggregator.Publish<NavigateHomeCommand>();
    }

    public class ActionItem
    {
        public string Text { get; set; }
        public ICommand Command { get; set; }
        public string Description { get; set; }
    }
    public class LinkItem
    {
        public string Text { get; set; }
        public string Uri { get; set; }
    }

}