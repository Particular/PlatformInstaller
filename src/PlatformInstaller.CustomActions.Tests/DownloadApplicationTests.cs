using System.Linq;

namespace PlatformInstaller.CustomActions.Tests
{
    using System.IO;
    using Microsoft.Deployment.WindowsInstaller;
    using NUnit.Framework;

    [TestFixture]
    public class DownloadApplicationTests : CustomActionContext
    {
        [Test, Explicit()]
        public void DownloadNServiceBus()
        {
            DownloadApp("NServiceBus");
        }

        
        [Test, Explicit()]
        public void DownloadServiceMatrix()
        {
            DownloadApp("ServiceMatrix");

        }

        [Test, Explicit()]
        public void DownloadServiceControl()
        {
            DownloadApp("ServiceControl");

        }


        [Test, Explicit()]
        public void DownloadServicePulse()
        {
            DownloadApp("ServicePulse");

        }

        ActionResult DownloadApp(string app)
        {
            var targetDir = CreateDirForTesting("Application-" + app);

            session["APPLICATION_NAME"] = app;
            session["TARGET_APP_DIR"] = targetDir;


            var result = CustomActions.DownloadApplication(null);

            Assert.AreEqual(ActionResult.Success, result);

            Assert.True(Directory.EnumerateFiles(targetDir)
                .Any(f => f.Contains("Particular." + app)));
            return result;
        }

    }
}