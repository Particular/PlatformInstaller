namespace PlatformInstaller.CustomActions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;

    public class CustomActionContext
    {
        protected string CreateDirForTesting(string name)
        {
            var targetDir = Path.Combine(Path.GetTempPath(), name);

      
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }

            Directory.CreateDirectory(targetDir);

            Console.Out.WriteLine(targetDir + " prepared");

            return targetDir;

        }

        [SetUp]
        public void SetUp()
        {
            session = new Dictionary<string, string>();

            CustomActions.LogAction = (session1, s) => { };
            CustomActions.SetAction = (session1, key, value) => { session[key] = value; };
            CustomActions.GetAction = (s, key) => session[key];
        }

        protected Dictionary<string, string> session;
    }
}