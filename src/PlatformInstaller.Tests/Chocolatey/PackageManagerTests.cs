using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class PackageManagerTests
{
    [Test]
    [Explicit("Integration")]
    public async void Install()
    {
        var packageInstaller = new PackageManager("Pester")
        {
            OutputDataReceived =  s=>Debug.WriteLine(s)
        };
        await packageInstaller.Install();
    }


    [Test]
    [Explicit("Integration")]
    public async void Uninstall()
    {

        var packageInstaller = new PackageManager("Pester")
        {
            OutputDataReceived = s => Debug.WriteLine(s)
        };
        await packageInstaller.Uninstall();
    }
}