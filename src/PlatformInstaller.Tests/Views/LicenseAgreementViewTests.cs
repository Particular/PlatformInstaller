using Autofac;
using NUnit.Framework;

[TestFixture]
public class LicenseAgreementViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void Show()
    {
        ShellViewModel.StartModel= ContainerFactory.Container.Resolve<LicenseAgreementViewModel>();
        var app = new App();
        app.Run();
    }

    [Test]
    [Explicit]
    [RequiresSTA]
    public void ScreenShot()
    {
        ContainerFactory.Container.Resolve<LicenseAgreementViewModel>().TakeScreenShot();
    }
}

