using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class FailedInstallationViewTests
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

    static FailedInstallationViewModel GetModel()
    {
        var failures = new List<string>
            {
                "Error1",
                "Error2"
            };
        return new FailedInstallationViewModel(new FakeEventAggregator(), "The failure reason", failures);
    }
}