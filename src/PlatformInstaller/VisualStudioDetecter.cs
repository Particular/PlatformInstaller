using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class VisualStudioDetecter
{
    public static bool VS2013Installed
    {
        get { return InstalledVersions.Contains(VisualStudioVersions.VS2013); }
    }

    public static bool VS2012Installed
    {
        get { return InstalledVersions.Contains(VisualStudioVersions.VS2012); }
    }

    public static bool VS2010Installed
    {
        get { return InstalledVersions.Contains(VisualStudioVersions.VS2010); }
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

        if (CheckVSTools("VS100COMNTOOLS"))
        {
            versions.Add(VisualStudioVersions.VS2010);
        }


        if (CheckVSTools("VS110COMNTOOLS"))
        {
            versions.Add(VisualStudioVersions.VS2012);
        }


        if (CheckVSTools("VS120COMNTOOLS"))
        {
            versions.Add(VisualStudioVersions.VS2013);
        }

        return versions;
    }

    static bool CheckVSTools(string environmentVariable)
    {
        var folderPath = Environment.GetEnvironmentVariable(environmentVariable);

        if (string.IsNullOrEmpty(folderPath))
        {
            return false;
        }

        var idePath = Path.Combine(folderPath, @"..\IDE");

        var pathToVsixInstaller = Path.Combine(idePath, "VSIXInstaller.exe");

        return File.Exists(pathToVsixInstaller);
    }

    static List<string> installedVersions;
}