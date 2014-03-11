using NUnit.Framework;

[TestFixture]
public class PackageManagerTests
{
    [Test]
    [Explicit("Integration")]
    public async void Install()
    {
        var packageInstaller = new PackageManager(new PowerShellRunner(new ProgressService()));
        await packageInstaller.Install("Pester");
    }


    [Test]
    [Explicit("Integration")]
    public async void Uninstall()
    {
        var packageInstaller = new PackageManager(new PowerShellRunner(new ProgressService()));
        await packageInstaller.Uninstall("Pester");
    }
}