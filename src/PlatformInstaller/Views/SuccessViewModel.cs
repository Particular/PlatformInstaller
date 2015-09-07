using System.Collections.Generic;
using System.Linq;
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
        Items = GetItems().ToList();
    }

    IEnumerable<Item> GetItems()
    {
        return from name in installedItemNames
            let installer = installers.Single(x => x.Name == name)
            from action in installer.GetAfterInstallActions()
            select new Item
            {
                Command = new SimpleCommand(action.Action),
                Text = action.Text
            };
    }

    public List<Item> Items { get; set; }
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

    public class Item 
    {
        public string Text { get; set; }
        public ICommand Command { get; set; }
    }

}