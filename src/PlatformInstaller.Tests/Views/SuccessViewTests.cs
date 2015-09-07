using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class SuccessViewTests
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

    static SuccessViewModel GetModel()
    {
        return new SuccessViewModel(null, null, null)
        {
            ActionItems = GetItems(),
            LinkItems = GetLinks()
        };
    }

    static List<SuccessViewModel.LinkItem> GetLinks()
    {
        return new List<SuccessViewModel.LinkItem>
        {
            new SuccessViewModel.LinkItem
            {
                Uri = "the url",
                Text = "Hello"
            }
        };

    }

    static List<SuccessViewModel.ActionItem> GetItems()
    {
        return new List<SuccessViewModel.ActionItem>
        {
            new SuccessViewModel.ActionItem
            {
                Command = new SimpleCommand(() => { }),
                Text = "The Title1",
                Description = "The Description1"
            },
            new SuccessViewModel.ActionItem
            {
                Command = new SimpleCommand(() => { }),
                Text = "The Title2",
                Description = "The Description2"
            },
        };
    }
}