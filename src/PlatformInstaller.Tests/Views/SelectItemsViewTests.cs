using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class SelectItemsViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void Show()
    {
        var model = GetModel();
        ViewTester.ShowView(model);
    }

    [Test]
    [RequiresSTA]
    [Explicit]
    public void Screenshot()
    {
        var model = GetModel();
        ViewTester.ScreenCapture(model);
    }

    static SelectItemsViewModel GetModel()
    {
        //todo: create a fake runner
        return new SelectItemsViewModel(new List<IInstaller>(), new FakeEventAggregator(), new FakePendingRestartAndResume(), null, null)
            {
                PackageDefinitions = GetPackages()
            };
    }

    static List<SelectItemsViewModel.PackageDefinitionBindable> GetPackages()
    {
        return new List<SelectItemsViewModel.PackageDefinitionBindable>
        {
            new SelectItemsViewModel.PackageDefinitionBindable
            {
                Name = "NServiceBusPreReqs",
                ImageUrl = ResourceResolver.GetPackUrl("/Images/NServiceBus Pre-requisites.png"),
                ToolTip = "NServiceBus",
                Enabled = true,
                Status = "The Status"
            },
            new SelectItemsViewModel.PackageDefinitionBindable
            {
                Name = "ServiceControl",
                ImageUrl = ResourceResolver.GetPackUrl("/Images/ServiceControl.png"),
                ToolTip = "ServiceControl",
                Enabled = true,
                Status = "The Status"
            },
        };
    }
}