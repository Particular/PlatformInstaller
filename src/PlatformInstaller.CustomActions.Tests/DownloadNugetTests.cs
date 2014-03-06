namespace PlatformInstaller.CustomActions.Tests
{
    using System.IO;
    using Microsoft.Deployment.WindowsInstaller;
    using NUnit.Framework;

    [TestFixture]
    public class DownloadNugetTests : CustomActionContext
    {
        [Test, Explicit()]
        public void DownloadNuGet()
        {
            var targetDir = CreateDirForTesting("Nugets");

            session["NUGET_NAME"] = "NServiceBus.Interfaces";
            session["TARGET_NUGET_DIR"] = targetDir;


            var result = CustomActions.DownloadNuget(null);

            Assert.AreEqual(ActionResult.Success, result);

            Assert.True(File.Exists(Path.Combine(targetDir, "NServiceBus.dll")));
        }
    }
}