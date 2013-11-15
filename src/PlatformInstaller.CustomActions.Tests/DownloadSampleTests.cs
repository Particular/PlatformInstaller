
namespace PlatformInstaller.CustomActions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Deployment.WindowsInstaller;
    using NUnit.Framework;

    [TestFixture]
    public class DownloadSampleTests
    {
        [Test, Explicit()]
        public void DownloadSample()
        {
            var targetDir = Path.Combine(Path.GetTempPath(),"Samples");

            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir,true);
            }

            Directory.CreateDirectory(targetDir);

            Console.Out.WriteLine(targetDir);

            var session = new Dictionary<string, string>
            {
                { "SAMPLE_REPOSITORY", "Particular/NServiceBus.SqlServer.Samples" },
                { "TARGET_SAMPLE_DIR", targetDir }
            };

            CustomActions.LogAction = (session1, s) => { };
            CustomActions.SetAction = (session1, key, value) => { session[key] = value; };
            CustomActions.GetAction = (s, key) => session[key];
            var result = CustomActions.DownloadSamples(null);

            Assert.AreEqual(ActionResult.Success, result);
        }
    }
}