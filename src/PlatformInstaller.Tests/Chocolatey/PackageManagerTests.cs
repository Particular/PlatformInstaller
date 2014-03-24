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
        var packageInstaller = GetPackageInstaller();
        await packageInstaller.Install("Pester");
    }
    [Test]
    [Explicit("Integration")]
    public void CopyLogFiles()
    {
        Logging.Initialise();
        PackageManager.CopyLogFiles("ServiceControl");
    }

    static PackageManager GetPackageInstaller()
    {
        var progressService = new ProgressService();
        var platformInstallerPsHostUi = new PlatformInstallerPSHostUI(progressService);
        var platformInstallerPsHost = new PlatformInstallerPSHost(platformInstallerPsHostUi);
        return new PackageManager(new PowerShellRunner(platformInstallerPsHost, progressService));
    }


    [Test]
    [Explicit("Integration")]
    public async void Uninstall()
    {
        var packageInstaller = GetPackageInstaller();
        await packageInstaller.Uninstall("Pester");
    }

    [Test]
    [Explicit("Integration")]
    public async void TryGetInstalledVersion()
    {
        var packageInstaller = GetPackageInstaller();
        SemanticVersion version;
        packageInstaller.TryGetInstalledVersion("Pester", out version);
        Debug.WriteLine(version);
    }
}