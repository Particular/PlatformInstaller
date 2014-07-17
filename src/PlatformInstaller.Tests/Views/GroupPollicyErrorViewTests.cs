using NUnit.Framework;

[TestFixture]
public class GroupPolicyErrorViewTests
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

    static GroupPolicyErrorViewModel GetModel()
    {
        return new GroupPolicyErrorViewModel(new FakeEventAggregator(), new PowerShellRunner());
    }
}