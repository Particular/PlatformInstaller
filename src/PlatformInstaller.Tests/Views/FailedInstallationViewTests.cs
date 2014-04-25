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
    
#if(DEBUG)
    [Test]
    [RequiresSTA]
    public void Verify()
    {
        var model = GetModel();
        ViewTester.VerifyView(model);
    }
#endif

    [Test]
    [RequiresSTA]
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