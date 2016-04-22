using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class ServiceControlInstallerTests
{
    [Test]
    [Explicit]
    public void GetManagementPath()
    {
        var managementPath = ServiceControlInstaller.GetManagementPath();
        Trace.WriteLine(managementPath);
    }
}