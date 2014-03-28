﻿using System.Diagnostics;
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
    public async void InstallWithParams()
    {
        var packageInstaller = GetPackageInstaller();
        await packageInstaller.Install("RavenDB", @"/quiet /log C:\raven_log.txt /msicl RAVEN_TARGET_ENVIRONMENT=DEVELOPMENT /msicl TARGETDIR=C:\ /msicl INSTALLFOLDER=C:\RavenDB /msicl RAVEN_INSTALLATION_TYPE=SERVICE /msicl REMOVE=IIS /msicl ADDLOCAL=Service");
    }
    
    [Test]
    [Explicit("Integration")]
    public void CopyLogFiles()
    {
        PackageManager.CopyLogFiles("ServiceControl");
    }

    static PackageManager GetPackageInstaller()
    {
        var progressService = new ProgressService();
        var platformInstallerPsHostUi = new PlatformInstallerPSHostUI(progressService);
        var platformInstallerPsHost = new PlatformInstallerPSHost(platformInstallerPsHostUi);
        return new PackageManager(new PowerShellRunner(platformInstallerPsHost, progressService), new ChocolateyInstaller(null));
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