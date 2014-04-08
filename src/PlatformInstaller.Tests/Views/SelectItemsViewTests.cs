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
        var selectItemsViewModel = SelectItemsViewModel();
        ShellViewModel.StartModel = selectItemsViewModel;
        var app = new App();
        app.Run();
    }

    static SelectItemsViewModel SelectItemsViewModel()
    {
        var selectItemsViewModel = ContainerFactory.Container.Resolve<SelectItemsViewModel>();
        foreach (var packageDefinitionBindable in selectItemsViewModel.PackageDefinitions)
        {
            packageDefinitionBindable.Status = "Install";
            packageDefinitionBindable.Selected = true;
            packageDefinitionBindable.Enabled = true;
        }
        return selectItemsViewModel;
    }

    [Test]
    [Explicit]
    [RequiresSTA]
    public void ScreenShot()
    {
        var selectItemsViewModel = SelectItemsViewModel();
        foreach (var definition in selectItemsViewModel.PackageDefinitions)
        {
            definition.Selected = true;
        }
        selectItemsViewModel.TakeScreenShot();
    }
}