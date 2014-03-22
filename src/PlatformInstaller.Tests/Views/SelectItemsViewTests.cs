using Autofac;
using NUnit.Framework;

[TestFixture]
public class SelectItemsViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void ShowDialog()
    {
        ShellViewModel.StartModel = ContainerFactory.Container.Resolve<SelectItemsViewModel>();
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

}