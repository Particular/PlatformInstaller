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
        return new SelectItemsViewModel(new FakeInstallationDefinitionService(), new FakeEventAggregator(), new FakePendingRestartAndResume(), null, null, null  )
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
                ImageUrl = ResourceResolver.GetPackUrl("/Images/NSB.png"),
                ToolTip = "NServiceBus",
                Enabled = true,
            },
            new SelectItemsViewModel.PackageDefinitionBindable
            {
                Name = "ServiceControl",
                ImageUrl = ResourceResolver.GetPackUrl("/Images/SC.png"),
                ToolTip = "ServiceControl",
                Enabled = true,
            },
            new SelectItemsViewModel.PackageDefinitionBindable
            {
                Name = "ServicePulse",
                ImageUrl = ResourceResolver.GetPackUrl("/Images/SP.png"),
                ToolTip = "ServicePulse",
                Enabled = true,
            },
            new SelectItemsViewModel.PackageDefinitionBindable
            {
                Name = "ServiceInsight",
                ImageUrl = ResourceResolver.GetPackUrl("/Images/SI.png"),
                ToolTip = "ServiceInsight",
                Enabled = true,
            },
        };
    }
}