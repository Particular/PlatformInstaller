using System.Management.Automation;
using NUnit.Framework;

[TestFixture]
public class PowerShellExtensionsTests
{
    [Test]
    public void ToDownloadingStringCanParse()
    {
        var progressRecord = new ProgressRecord(0, "foo", "Saving 44409 of 21223512");
        Assert.AreEqual("43.4KB of 20.2MB", progressRecord.ToDownloadingString());
    }

    [Test]
    public void ToDownloadingStringCannotParse()
    {
        var progressRecord = new ProgressRecord(0, "foo", "44409 of 21223512");
        Assert.AreEqual("44409 of 21223512", progressRecord.ToDownloadingString());
    }

}