using NUnit.Framework;

[TestFixture]
public class ChocolateyInstallerTests
{
    [Test]
    [Explicit("Integration")]
    public void Install()
    {
        var packageInstaller = new ChocolateyInstaller();
        packageInstaller.InstallChocolatey().Wait();
    }

}