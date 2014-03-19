using Caliburn.Micro;
using NUnit.Framework;

[TestFixture]
public class InstallChocolateyViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void ShowDialog()
    {
        ViewLocator.LocateForModel = (o, dependencyObject, arg3) => new InstallChocolateyView();
        var windowManager = new WindowManager();
        windowManager.ShowDialog(new InstallChocolateyViewModel());
    }
}