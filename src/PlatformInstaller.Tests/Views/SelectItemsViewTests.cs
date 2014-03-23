using Autofac;
using NUnit.Framework;

[TestFixture]
public class SelectItemsViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void Show()
    {
        ShellViewModel.StartModel = ContainerFactory.Container.Resolve<SelectItemsViewModel>();
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

}