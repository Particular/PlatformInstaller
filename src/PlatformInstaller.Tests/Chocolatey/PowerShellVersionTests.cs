using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class PowerShellVersionTests
{
    [Test]
    public void Initialise()
    {
        var packageInstaller = new PowerShellVersion();
        packageInstaller.Initialise();
        Debug.WriteLine(packageInstaller.Version);
    }

}