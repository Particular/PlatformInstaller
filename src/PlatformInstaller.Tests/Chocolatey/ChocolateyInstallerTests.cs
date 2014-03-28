using NUnit.Framework;

[TestFixture]
public class ChocolateyInstallerTests
{
    [Test]
    [Explicit("Integration")]
    public async void Install()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(new ProgressService()));
        await chocolateyInstaller.InstallChocolatey();
        Assert.IsTrue(chocolateyInstaller.IsInstalled());
    }
    [Test]
    [Explicit("Integration")]
    public void IsInstalled()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(new ProgressService()));
        Assert.IsTrue(chocolateyInstaller.IsInstalled());
    }
    [Test]
    [Explicit("Integration")]
    public void GetInstallPath()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(new ProgressService()));
        Assert.AreEqual(@"C:\Chocolatey", chocolateyInstaller.GetInstallPath());
    }

}