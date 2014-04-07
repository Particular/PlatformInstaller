using Autofac;
using NUnit.Framework;

[TestFixture]
public class UpdateChocolateyViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void Show()
    {
        ShellViewModel.StartModel = ContainerFactory.Container.Resolve<UpdateChocolateyViewModel>();
        var app = new App();
        app.Run();
    }

    [Test]
    [Explicit]
    [RequiresSTA]
    public void ScreenShot()
    {
        ContainerFactory.Container.Resolve<UpdateChocolateyViewModel>().TakeScreenShot();
    }
}