using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class ChocolateyInstallerTests
{
    [Test]
    [Explicit("Integration")]
    public async void Install()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(), new PowerShellRunner());
        await chocolateyInstaller.InstallChocolatey(null,null);
        Assert.IsTrue(chocolateyInstaller.IsInstalled());
    }
    [Test]
    [Explicit("Integration")]
    public void IsInstalled()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(), new PowerShellRunner());
        Assert.IsTrue(chocolateyInstaller.IsInstalled());
    }
    [Test]
    [Explicit("Integration")]
    public void GetInstallPath()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(), new PowerShellRunner());
        Assert.AreEqual(@"C:\Chocolatey", chocolateyInstaller.GetInstallPath());
    }

    [Test]
    [Explicit("Integration")]
    public async void GetInstallVersion()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(),new PowerShellRunner());
        var installVersion = await chocolateyInstaller.GetInstallVersion();
        Debug.WriteLine(installVersion);
        Assert.IsNotNull(installVersion);
    }

}