using NUnit.Framework;

[TestFixture]
public class InstallChocolateyViewTests
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

    static InstallChocolateyViewModel GetModel()
    {
        return new InstallChocolateyViewModel(new FakeEventAggregator(), new FakeChocolateyInstaller());
    }
}