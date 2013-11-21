
namespace PlatformInstaller.CustomActions.Tests
{
    using Microsoft.Deployment.WindowsInstaller;
    using NUnit.Framework;

    [TestFixture]
    public class DownloadSampleTests : CustomActionContext
    {
        [Test, Explicit()]
        public void DownloadSample()
        {
            var targetDir = CreateDirForTesting("Samples");

            session["SAMPLE_APPLICATION"] = "nservicebus";
            session["TARGET_SAMPLE_DIR"] = targetDir;


            var result = CustomActions.DownloadSamples(null);

            Assert.AreEqual(ActionResult.Success, result);
        }
    }
}