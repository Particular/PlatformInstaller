using NUnit.Framework;

[TestFixture]
public class VersionFinderTests
{
    [Test]
    public void VerifyCanGetVersion()
    {
        Assert.IsNotNullOrEmpty(VersionFinder.GetVersion());        
    }
}