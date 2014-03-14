using System.Diagnostics;
using NuGet;
using NUnit.Framework;

[TestFixture]
public class PackageManagerTests
{
    [Test]
    [Explicit("Integration")]
    public async void Install()
    {
        var packageInstaller = new PackageManager(new PowerShellRunner(new PlatformInstallerPSHost(new PlatformInstallerPSHostUI(new ProgressService()))));
        await packageInstaller.Install("Pester");
    }


    [Test]
    [Explicit("Integration")]
    public async void Uninstall()
    {
        var packageInstaller = new PackageManager(new PowerShellRunner(new PlatformInstallerPSHost(new PlatformInstallerPSHostUI(new ProgressService()))));
        await packageInstaller.Uninstall("Pester");
    }

    [Test]
    [Explicit("Integration")]
    public async void TryGetInstalledVersion()
    {
        var packageInstaller = new PackageManager(new PowerShellRunner(new PlatformInstallerPSHost(new PlatformInstallerPSHostUI(new ProgressService()))));
        SemanticVersion version;
        packageInstaller.TryGetInstalledVersion("Pester", out version);
        Debug.WriteLine(version);
    }
}