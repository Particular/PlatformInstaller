namespace PlatformInstaller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VisualStudioDetecter
    {
        public static bool VS2013Installed
        {
            get { return InstalledVersions.Contains("VS2013"); }
        }
        public static bool VS2012Installed
        {
            get { return InstalledVersions.Contains("VS2012"); }
        }
        public static bool VS2010Installed
        {
            get { return InstalledVersions.Contains("VS2010"); }
        }

        public static IEnumerable<string> InstalledVersions
        {
            get
            {
                if (installedVersions == null)
                {
                    installedVersions = DetectVersions();
                }

                return installedVersions;
            }
        }

        static List<string> DetectVersions()
        {
            var versions = new List<string>();
            
            var explicitVersions = Environment.GetCommandLineArgs().SingleOrDefault(arg => arg.StartsWith("VsVersions="));


            if (!string.IsNullOrEmpty(explicitVersions))
            {
                var values = explicitVersions.Substring(explicitVersions.IndexOf("=") + 1);

                return values.Split(';').ToList();
            }

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VS100COMNTOOLS")))
            {
                versions.Add("VS2010");
            }


            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VS110COMNTOOLS")))
            {
                versions.Add("VS2012");
            }


            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VS120COMNTOOLS")))
            {
                versions.Add("VS2013");
            }

            return versions;
        }

        static List<string> installedVersions;
    }
}