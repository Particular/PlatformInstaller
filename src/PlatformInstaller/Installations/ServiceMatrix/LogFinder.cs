namespace PlatformInstaller.Installations.ServiceMatrix
{
    using System;
    using System.IO;
    using System.Linq;

    public class LogFinder
    {
        public static string FindVSIXLog(string vsVersion)
        {
            var temp = Environment.GetEnvironmentVariable("temp");
            if (temp != null)
            {
                var tempInfo = new DirectoryInfo(temp);
                foreach (var f in tempInfo.GetFiles("vsixinstaller_*.log").OrderByDescending(p => p.CreationTime))
                {
                    if (FileIsMatrixVersion(f) == vsVersion)
                    {
                        return f.FullName;
                    }
                }
            }
            return null;
        }

        static string FileIsMatrixVersion(FileInfo f)
        {
            var content = f.OpenText().ReadToEnd();
            if (!content.Contains("ServiceMatrix")) return null;
            if (content.Contains("EE4496BE-1C92-42DB-B5FA-FB1B3AA306D0"))
            {
                return VisualStudioVersions.VS2013;
            }
            if (content.Contains("23795EC3-3DEA-4F04-9044-4056CF91A2ED"))
            {
                return VisualStudioVersions.VS2012;
            }
            return null;
        }
    }
}
