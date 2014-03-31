using NUnit.Framework;

[TestFixture]
public class ChocolateyInstallerTests
{
    [Test]
    [Explicit("Integration")]
    public async void Install()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner());
        await chocolateyInstaller.InstallChocolatey(null,null);
        Assert.IsTrue(chocolateyInstaller.IsInstalled());
    }
    [Test]
    [Explicit("Integration")]
    public void IsInstalled()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner());
        Assert.IsTrue(chocolateyInstaller.IsInstalled());
    }
    [Test]
    [Explicit("Integration")]
    public void GetInstallPath()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner());
        Assert.AreEqual(@"C:\Chocolatey", chocolateyInstaller.GetInstallPath());
    }

}