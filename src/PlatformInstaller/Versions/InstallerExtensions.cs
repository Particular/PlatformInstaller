using System;

public static class InstallerExtensions
{
    public static string ExeInstallerStatus(this IInstaller runner)
    {
        try
        {
            var current = runner.CurrentVersion();
            var latest = runner.LatestAvailableVersion();

            if (latest == null)
            {
                return null;
            }

            if (current == null)
            {
                return string.Format("Install {0}", latest);
            }

            if (current != latest)
            {
                return string.Format("Upgrade {0} to {1}",current, latest);
            }
            return string.Format("Latest is installed - {0}", current);
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }

    public static string VsixInstallerStatus(this IInstaller runner, string version)
    {
        var latest = runner.LatestAvailableVersion();
        var current = runner.CurrentVersion();

        switch (version)
        {
            case VisualStudioVersions.VS2012:
                if (!VisualStudioDetecter.VS2012Installed)
                {
                    return "Requires VS 2012";
                }
                break;
            case VisualStudioVersions.VS2013:
                if (!VisualStudioDetecter.VS2013Installed)
                {
                    return "Requires VS 2013";
                }
                break;
        }
        if (runner.Installed() && (latest == current))
        {
            return string.Format("Latest is installed - {0}", current);
        }
        if (runner.Installed() && (latest != current))
        {
            return string.Format("Update to {0} in Visual Studio", latest);
        }
        
        return  (latest == null) ? "" : string.Format("Install {0}", latest);
    }
}

