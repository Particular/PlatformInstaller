using NUnit.Framework;

[TestFixture]
public class ServiceMatrixPackageTests
{
    [TestCase(VisualStudioVersions.VS2013, true, true, "Already Installed", true, "ServiceMatrix for VS2013 is already installed")]
    [TestCase(VisualStudioVersions.VS2013, true, false, "Install", false, "Install ServiceMatrix for VS2013")]
    [TestCase(VisualStudioVersions.VS2013, false, false, "VS2013 Required", true, "This option requires VS2013")]
    [TestCase(VisualStudioVersions.VS2012, true, true, "Already Installed", true, "ServiceMatrix for VS2012 is already installed")]
    [TestCase(VisualStudioVersions.VS2012, true, false, "Install", false, "Install ServiceMatrix for VS2012")]
    [TestCase(VisualStudioVersions.VS2012, false, false, "VS2012 Required", true, "This option requires VS2012")]
    public void When_Visual_Studio_Is_Installed_Test(string visualStudioVersion,
        bool isVisualStudioInstalled,
        bool isChocolateyPackageInstalled,
        string expectedStatus,
        bool checkBoxShouldBeDisabled,
        string expectedTooltip)
    {
        var packageName = string.Format("ServiceMatrix.{0}.install", VisualStudioVersions.VS2012);
        var installDefinition = ServiceMatrix.GetInstallationDefinition(visualStudioVersion, isVisualStudioInstalled, isChocolateyPackageInstalled, packageName);
        Assert.AreEqual(expectedStatus, installDefinition.Status);
        Assert.AreEqual(expectedTooltip, installDefinition.ToolTip);
        Assert.AreEqual(checkBoxShouldBeDisabled, installDefinition.Disabled);
    }

    [Test, ExpectedException]
    public void When_Unsupported_Version_Of_Visual_Studio_Is_Installed()
    {
        var packageName = string.Format("ServiceMatrix.{0}.install", VisualStudioVersions.VS2010);
        ServiceMatrix.GetInstallationDefinition(VisualStudioVersions.VS2010, true, false, packageName);
    }
}