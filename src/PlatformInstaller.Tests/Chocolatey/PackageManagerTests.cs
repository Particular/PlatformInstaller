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
        await packageInstaller.Install("Pester", null,null,null,null,null);
    }
    [Test]
    [Explicit("Integration")]
    public async void InstallWithParams()
    {
        var packageInstaller = GetPackageInstaller();
        var installArguments = @"/quiet /log C:\raven_log.txt /msicl RAVEN_TARGET_ENVIRONMENT=DEVELOPMENT /msicl TARGETDIR=C:\ /msicl INSTALLFOLDER=C:\RavenDB /msicl RAVEN_INSTALLATION_TYPE=SERVICE /msicl REMOVE=IIS /msicl ADDLOCAL=Service";
        await packageInstaller.Install("RavenDB", installArguments, null, null, null, null);
    }
    
    [Test]
    [Explicit("Integration")]
    public void CopyLogFiles()
    {
        PackageManager.CopyLogFiles("ServiceControl");
    }

    static PackageManager GetPackageInstaller()
    {
        return new PackageManager(new PowerShellRunner(), new ChocolateyInstaller(null,null));
    }


    [Test]
    [Explicit("Integration")]
    public async void Uninstall()
    {
        var packageInstaller = GetPackageInstaller();
        await packageInstaller.Uninstall("Pester", null, null, null, null);
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