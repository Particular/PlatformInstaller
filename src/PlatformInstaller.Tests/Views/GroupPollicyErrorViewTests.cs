using NUnit.Framework;

[TestFixture]
public class GroupPollicyErrorViewTests
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
    public void ScreenShot()
    {
        var model = GetModel();
        ViewTester.ScreenCapture(model);
    }

    static GroupPollicyErrorViewModel GetModel()
    {
        return new GroupPollicyErrorViewModel(new FakeEventAggregator(), new PowerShellRunner());
    }
}