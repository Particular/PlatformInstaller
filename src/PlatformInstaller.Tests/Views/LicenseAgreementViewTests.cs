using Autofac;
using NUnit.Framework;

[TestFixture]
public class LicenseAgreementViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void ShowDialog()
    {
        ShellViewModel.StartModel= ContainerFactory.Container.Resolve<LicenseAgreementViewModel>();
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

}

