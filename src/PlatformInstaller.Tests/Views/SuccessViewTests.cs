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
        return new SuccessViewModel(null,null,null)
            {
                Items = GetItems()
            };
    }

    static List<SuccessViewModel.Item> GetItems()
    {
        return new List<SuccessViewModel.Item>
        {
            new SuccessViewModel.Item
            {
                Command = new SimpleCommand(() => {}),
                Text = "Item1",
            },
            new SuccessViewModel.Item
            {
                Command = new SimpleCommand(() => {}),
                Text = "Item2",
            },
        };
    }
}