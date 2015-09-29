public static class InstallerExtensions
{
    public static string ExeInstallerStatus(this IInstaller runner)
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
            return string.Format("Upgrade {0} to {1}", current, latest);
        }
        return string.Format("Latest is installed - {0}", current);
    }
}

