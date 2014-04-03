using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class ChocolateyInstallerTests
{
    [Test]
    [Explicit("Integration")]
    public async void Install()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(), new PowerShellRunner());
        await chocolateyInstaller.InstallChocolatey(null, null);
        Assert.IsTrue(chocolateyInstaller.IsInstalled());
    }

    [Test]
    [Explicit("Integration")]
    public void IsInstalled()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(), new PowerShellRunner());
        Assert.IsTrue(chocolateyInstaller.IsInstalled());
    }

    [Test]
    public void GetInstallPath()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(), new PowerShellRunner());
        Assert.AreEqual(@"C:\Chocolatey", chocolateyInstaller.GetInstallPath());
    }

    [Test]
    [Explicit("Integration")]
    public async void GetVersionFromHelpOutput()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(), new PowerShellRunner());
        var installVersion = await chocolateyInstaller.GetVersionFromHelpOutput();
        Debug.WriteLine(installVersion);
        Assert.IsNotNull(installVersion);
    }

    [Test]
    public void ParseVersionFromHelpOutput()
    {
        var version = ChocolateyInstaller.ParseVersionFromHelpOutput(new List<string>
            {
                "Please run chocolatey /? or chocolatey help - chocolatey v0.9.8.23",
                "Reading environment variables from registry. Please wait... Done."
            });
        Assert.AreEqual(new Version(0, 9, 8, 23), version);
        var missingVersion = ChocolateyInstaller.ParseVersionFromHelpOutput(new List<string>
            {
                "Please run chocolatey /? or chocolatey help",
                "Reading environment variables from registry. Please wait... Done."
            });
        Assert.IsNull(missingVersion);
    }

    [Test]
    [Explicit("Integration")]
    public async void GetVersionFromRawFile()
    {
        var chocolateyInstaller = new ChocolateyInstaller(new ProcessRunner(), new PowerShellRunner());
        var installVersion = await chocolateyInstaller.GetVersionFromRawFile();
        Debug.WriteLine(installVersion);
        Assert.IsNotNull(installVersion);
    }

    [Test]
    public void ParseVersionFromRawFile()
    {
        var sourceDirectory = Path.Combine(AssemblyLocation.CurrentDirectory, @"Chocolatey\ChocolateyPs1");
        foreach (var chocolateyFile in Directory.EnumerateFiles(sourceDirectory))
        {
            var version = ChocolateyInstaller.ParseVersionFromRawFile(chocolateyFile);
            Debug.WriteLine(Path.GetFileNameWithoutExtension(chocolateyFile) +"=" +  version);
            Assert.IsNotNull(version);
        }
    }
}