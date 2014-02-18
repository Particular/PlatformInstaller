public class PackageInstaller
{
    PowerShellRunner powerShellRunner;

    public PackageInstaller(PowerShellRunner powerShellRunner)
    {
        this.powerShellRunner = powerShellRunner;
    }

    //public void InstallPackage(string packageName)
    //{
    //    powerShellRunner.Run("cinst " + packageName);
    //}

    //public void UninstallPackage(string packageName)
    //{
    //    powerShellRunner.Run("cuninst " + packageName);
    //}
}