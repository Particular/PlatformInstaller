using NUnit.Framework;

[TestFixture]
public class StringExtensionTests
{
    [Test]
    public void SecureString()
    {
        Assert.AreEqual("Pass", "Pass".ToSecureString().ToOriginalString());
    }
}