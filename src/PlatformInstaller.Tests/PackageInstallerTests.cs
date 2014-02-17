using NUnit.Framework;

[Explicit("Integration")]
[TestFixture]
public class PackageInstallerTests
{
    [Test]
    public void Install()
    {
        var packageInstaller = new PackageInstaller(new PowerShellRunner());
      //  packageInstaller .InstallPackage();
    }

    [Test]
    public void Uninstall()
    {
        
    }
}