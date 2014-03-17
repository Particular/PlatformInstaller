namespace PlatformInstaller.Tests
{
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
            ViewLocator.LocateForModel = (o, dependencyObject, arg3) =>
            {
                return new InstallChocolateyView();
            };
            var windowManager = new WindowManager();
            windowManager.ShowDialog(new InstallChocolateyViewModel());
          //  new InstallChocolateyView(){DataContext = new InstallChocolateyViewModel()}.ShowDialog();
        }
    }
}