using NUnit.Framework;

[TestFixture]
public class PackageManagerTests
{
    [Test]
    [Explicit("Integration")]
    public async void Install()
    {
        var packageInstaller = new PackageManager(new PowerShellRunner(new PlatformInstallerPSHost(new PlatformInstallerPSHostUI(new ProgressService()))));
        await packageInstaller.Install("Pester", "Pester");
    }


    [Test]
    [Explicit("Integration")]
    public async void Uninstall()
    {
        var packageInstaller = new PackageManager(new PowerShellRunner(new PlatformInstallerPSHost(new PlatformInstallerPSHostUI(new ProgressService()))));
        await packageInstaller.Uninstall("Pester");
    }
}