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
            return $"Install {latest}";
        }

        if (current != latest)
        {
            return $"Upgrade {current} to {latest}";
        }
        return $"Latest is installed - {current}";
    }

    public static InstallState ExeInstallState(this IInstaller runner)
    {
        var current = runner.CurrentVersion();
        var latest = runner.LatestAvailableVersion();
        if (current == null)
        {
            return InstallState.NotInstalled;
        }

        if (current != latest)
        {
            return InstallState.UpgradeAvailable;
        }
        return InstallState.Installed;
    }
}

