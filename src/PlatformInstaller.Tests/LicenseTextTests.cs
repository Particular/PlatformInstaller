using ApprovalTests;
using NUnit.Framework;

[TestFixture]
public class LicenseTextTests
{
    [Test]
    public void VerifyLicenseHtml()
    {
        Approvals.Verify(LicenseText.ReadLicenseHtml());
    }
}