using NUnit.Framework;

[TestFixture]
public class DotNetInstallFailedViewTests
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

    static DotNetInstallFailedViewModel GetModel()
    {
        return new DotNetInstallFailedViewModel();
    }
}