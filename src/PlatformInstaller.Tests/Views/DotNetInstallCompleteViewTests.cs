using NUnit.Framework;

[TestFixture]
public class DotNetInstallCompleteViewTests
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

    static DotNetInstallCompleteViewModel GetModel()
    {
        return new DotNetInstallCompleteViewModel();
    }
}