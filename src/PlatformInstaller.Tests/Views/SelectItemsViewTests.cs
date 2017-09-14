using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

[TestFixture]
public class SelectItemsViewTests
{
    [Test]
    [Explicit]
    [Apartment(ApartmentState.STA)]
    public void Show()
    {
        var model = GetModel();
        ViewTester.ShowView(model);
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    [Explicit]
    public void Screenshot()
    {
        var model = GetModel();
        ViewTester.ScreenCapture(model);
    }

    static SelectItemsViewModel GetModel()
    {
        //todo: create a fake runner
        return new SelectItemsViewModel(new List<IInstaller>(), new FakeEventAggregator())
            {
                Items = GetItems()
            };
    }

    static List<SelectItemsViewModel.Item> GetItems()
    {
        return new List<SelectItemsViewModel.Item>
        {
            new SelectItemsViewModel.Item
            {
                Name = "NServiceBusPreReqs",
                ImageUrl = ResourceResolver.GetPackUrl("/Images/NServiceBus Pre-requisites.png"),
                ToolTip = "NServiceBus",
                Status = "The Status"
            },
            new SelectItemsViewModel.Item
            {
                Name = "ServiceControl",
                ImageUrl = ResourceResolver.GetPackUrl("/Images/ServiceControl.png"),
                ToolTip = "ServiceControl",
                Status = "The Status"
            },
        };
    }
}