using System.Net;
using NUnit.Framework;

[TestFixture]
public class ProxyTesterTest
{
    [Test]
    [Explicit]
    public void ProxyTest()
    {
        var proxyTester = new ProxyTester(new CredentialStore());
        Assert.IsTrue(proxyTester.TestCredentials(CredentialCache.DefaultCredentials));
        Assert.IsTrue(proxyTester.TestCredentials(CredentialCache.DefaultNetworkCredentials));
    }
}