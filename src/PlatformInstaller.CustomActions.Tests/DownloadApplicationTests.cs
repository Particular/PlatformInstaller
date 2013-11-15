namespace PlatformInstaller.CustomActions.Tests
{
    using System.IO;
    using Microsoft.Deployment.WindowsInstaller;
    using NUnit.Framework;

    [TestFixture]
    public class DownloadApplicationTests : CustomActionContext
    {
        [Test, Explicit()]
        public void DownloadApplication()
        {
            var targetDir = CreateDirForTesting("Applications");

            session["APPLICATION_NAME"] = "Console2";//just for testing
            session["TARGET_APP_DIR"] = targetDir;


            var result = CustomActions.DownloadApplication(null);

            Assert.AreEqual(ActionResult.Success, result);

            Assert.True(File.Exists(Path.Combine(targetDir, "Console.exe")));
        }
    }
}