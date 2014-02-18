using NUnit.Framework;

[TestFixture]
public class PackageInstallerTests
{
    [Test]
    [Explicit("Integration")]
    public void Install()
    {
       var packageInstaller = new PackageInstaller(new PowerShellRunner());
       packageInstaller.InstallPackage("Pester");
    }

    [Test]
    [Explicit("Integration")]
    public void Uninstall()
    {
        
    }
}