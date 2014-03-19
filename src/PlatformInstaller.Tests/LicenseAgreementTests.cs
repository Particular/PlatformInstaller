using NUnit.Framework;

[TestFixture]
public class LicenseAgreementTests
{
    [Test]
    public void Verify()
    {
        var licenseAgreement = new LicenseAgreement();
        try
        {
            licenseAgreement.Agree();
            Assert.IsTrue(licenseAgreement.HasAgreedToLicense());
            licenseAgreement.Clear();
            Assert.IsFalse(licenseAgreement.HasAgreedToLicense());
        }
        finally
        {
            licenseAgreement.Clear();
        }
    }
}