using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class PackageInstallerTests
{
    [Test]
    [Explicit("Integration")]
    public void Install()
    {
       // var powerShellRunner = new PowerShellRunner { OutputDataReceived = Foo };
       // var packageInstaller = new PackageInstaller(powerShellRunner);
       //packageInstaller.InstallPackage("Pester");
    }

    void Foo(string obj)
    {
        Debug.WriteLine(obj);

    }

    [Test]
    [Explicit("Integration")]
    public void Uninstall()
    {
        
    }
}