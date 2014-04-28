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