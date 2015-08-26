using System.Net;
using NUnit.Framework;

[TestFixture]
public class ProxyTesterTest
{
    [Test]
    [Explicit]
    public void ProxyTest()
    {
        Assert.IsTrue(ProxyTester.ProxyTest(CredentialCache.DefaultCredentials));
        Assert.IsTrue(ProxyTester.ProxyTest(CredentialCache.DefaultNetworkCredentials));
    }
}