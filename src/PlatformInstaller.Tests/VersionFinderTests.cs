using NUnit.Framework;

[TestFixture]
public class VersionFinderTests
{
    [Test]
    public void VerifyCanGetVersion()
    {
        Assert.IsFalse(string.IsNullOrEmpty(VersionFinder.GetVersion()));
    }
}