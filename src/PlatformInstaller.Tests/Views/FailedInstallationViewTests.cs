using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

[TestFixture]
public class FailedInstallationViewTests
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