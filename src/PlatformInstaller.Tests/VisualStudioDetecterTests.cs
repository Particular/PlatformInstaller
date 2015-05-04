using System.Linq;
using NUnit.Framework;

[TestFixture]
public class VisualStudioDetecterTests
{
    [Test]
    [Explicit]
    public void VerifyVersionsInstalled()
    {
        Assert.True(VisualStudioDetecter.InstalledVersions.Contains("VS2013"));
        Assert.True(VisualStudioDetecter.VS2013Installed);

        Assert.False(VisualStudioDetecter.VS2012Installed);
    }
}