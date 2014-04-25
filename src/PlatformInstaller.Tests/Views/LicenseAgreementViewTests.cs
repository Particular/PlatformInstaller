using NUnit.Framework;

[TestFixture]
public class LicenseAgreementViewTests
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

    static LicenseAgreementViewModel GetModel()
    {
        return new LicenseAgreementViewModel(new FakeEventAggregator());
    }
}

