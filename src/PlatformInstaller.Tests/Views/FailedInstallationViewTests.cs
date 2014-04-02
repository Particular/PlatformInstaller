using System.Collections.Generic;
using Autofac;
using NUnit.Framework;

[TestFixture]
public class FailedInstallationViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void Show()
    {
        var failureReason = new NamedParameter("failureReason","The failure reason");
        var failures = new NamedParameter("failures",new List<string>{"Error1", "Error2"});
        ShellViewModel.StartModel = ContainerFactory.Container.Resolve<FailedInstallationViewModel>(failureReason, failures);
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

    [Test]
    [Explicit]
    [RequiresSTA]
    public void ScreenShot()
    {
        var failureReason = new NamedParameter("failureReason", "The failure reason");
        var failures = new NamedParameter("failures", new List<string> { "Error1", "Error2" });
        ContainerFactory.Container.Resolve<FailedInstallationViewModel>(failureReason, failures).TakeScreenShot();
    }
}