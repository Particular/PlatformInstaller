using Caliburn.Micro;
using NUnit.Framework;
using PlatformInstaller;

[TestFixture]
public class SelectItemsViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void ShowDialog()
    {
        ViewLocator.LocateForModel = (o, dependencyObject, arg3) => new SelectItemsView();
        var windowManager = new WindowManager();
        windowManager.ShowDialog(new SelectItemsViewModel(new FakePackageDefinitionService(), new FakeEvenAggregator()));
    }

}