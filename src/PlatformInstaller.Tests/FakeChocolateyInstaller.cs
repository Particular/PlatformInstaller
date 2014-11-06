using System;
using System.Threading.Tasks;

public class FakeChocolateyInstaller : ChocolateyInstaller
{
    public FakeChocolateyInstaller() : base(null, null)
    {
    }

    public override Task<bool> ChocolateyUpgradeRequired()
    {
        return  new Task<bool>(() => false);
    }

    public override string GetChocolateyPs1Path()
    {
        return null;
    }

    public override string GetInstallPath()
    {
        return null;
    }

    public override Task<Version> GetInstalledVersion()
    {
        return new Task<Version>(() => new Version(1, 1, 1, 1));
    }

    public override Task<Version> GetVersionFromHelpOutput()
    {
        return new Task<Version>(() => new Version(1, 1, 1, 1));
    }

    public override Task<Version> GetVersionFromRawFile()
    {
        return new Task<Version>(() => new Version(1,1,1,1) );
    }

    public override Task<int> InstallChocolatey(Action<string> outputAction, Action<string> errorAction)
    {
        return new Task<int>(() => 0);
    }

    public override bool IsInstalled()
    {
        return true;
    }

}