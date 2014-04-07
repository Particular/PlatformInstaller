using Autofac;
using NUnit.Framework;

[TestFixture]
public class InstallChocolateyViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void Show()
    {
        ShellViewModel.StartModel = ContainerFactory.Container.Resolve<InstallChocolateyViewModel>();
        var app = new App();
        app.Run();
    }

    [Test]
    [Explicit]
    [RequiresSTA]
    public void ScreenShot()
    {
        ContainerFactory.Container.Resolve<InstallChocolateyViewModel>().TakeScreenShot();
    }
}