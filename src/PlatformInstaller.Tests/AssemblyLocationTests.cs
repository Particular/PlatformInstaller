using System.IO;
using NUnit.Framework;


[TestFixture]
public class AssemblyLocationTests
{
    [Test]
    public void Verify()
    {
        Assert.True(File.Exists(AssemblyLocation.ExeFilePath));
        Assert.AreEqual("PlatformInstaller",AssemblyLocation.ExeFileName);
        Assert.True(Directory.Exists(AssemblyLocation.CurrentDirectory));
    }
}