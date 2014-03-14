using NUnit.Framework;

[TestFixture]
public class ChocolateyInstallerTests
{
    [Test]
    [Explicit("Integration")]
    public async void Install()
    {
        var chocolateyInstaller = new ChocolateyInstaller();
        await chocolateyInstaller.InstallChocolatey();
        Assert.IsTrue(chocolateyInstaller.IsInstalled());
    }
    [Test]
    [Explicit("Integration")]
    public async void IsInstalled()
    {
        var chocolateyInstaller = new ChocolateyInstaller();
        Assert.IsTrue(chocolateyInstaller.IsInstalled());
    }

}