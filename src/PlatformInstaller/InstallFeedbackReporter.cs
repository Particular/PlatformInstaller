using System.Linq;
using Anotar.Serilog;
using Caliburn.Micro;
using Microsoft.Win32;

public class InstallFeedbackReporter : IHandle<InstallSucceededEvent>, IHandle<InstallCancelledEvent>, IHandle<NoInstallAttemptedEvent>
{
    bool isNewUserAtStartup;

    const string cancelledUrl = @"http://particular.net/platform-installation-cancelled";

    public InstallFeedbackReporter()
    {
        isNewUserAtStartup = IsNewUser();
    }

    public void Handle(InstallSucceededEvent message)
    {
        if (HasFeebackBeenReportedForThisMachine())
        {
            LogTo.Information("Install feedback has already been reported no new browser will be popped");
            return;
        }
        LogTo.Information("Install successfull, new user: " + isNewUserAtStartup);
        var isNew = isNewUserAtStartup.ToString().ToLower();
        var installed = string.Join(";", message.InstalledItems);
        var url = $@"http://particular.net/thank-you-for-installing-the-particular-service-platform?new_user={isNew}&installed={installed}&nuget={NugetFlag()}";
        RecordInstallationFeeback();
        UrlLauncher.Open(url);
    }

    public void Handle(InstallCancelledEvent message)
    {
        // Show the feedback page on every cancelled install
        RecordInstallationFeeback();
        UrlLauncher.Open(cancelledUrl);
    }

    public void Handle(NoInstallAttemptedEvent message)
    {
        // Show the feedback page on first run if they close without installing stuff
        if (HasFeebackBeenReportedForThisMachine())
        {
            return;
        }
        RecordInstallationFeeback();
        UrlLauncher.Open(cancelledUrl);
    }

    void RecordInstallationFeeback()
    {
        using (var regRoot = Registry.CurrentUser.CreateSubKey(@"Software\ParticularSoftware\PlatformInstaller\"))
        {
            // ReSharper disable once PossibleNullReferenceException
            regRoot.SetValue("InstallationFeedbackReported", "true");
        }
    }

    bool HasFeebackBeenReportedForThisMachine()
    {
        using (var regRoot = Registry.CurrentUser.OpenSubKey(@"Software\ParticularSoftware\PlatformInstaller\"))
        {
            return regRoot?.GetValue("InstallationFeedbackReported") != null;
        }
    }

    public bool IsNewUser()
    {
        using (var regRoot = Registry.CurrentUser.OpenSubKey("Software"))
        {
            return CheckRegistryForExistingKeys(regRoot);
        }
    }

    string NugetFlag()
    {
        using (var regRoot = Registry.CurrentUser.OpenSubKey(@"Software\ParticularSoftware"))
        {
            if (regRoot != null)
            {
                return (string) regRoot.GetValue("NuGetUser", "false");
            }
        }
        return "false";
    }


    public static bool CheckRegistryForExistingKeys(RegistryKey regRoot)
    {
        return regRoot.GetSubKeyNames().All(subKey =>
            subKey != "NServiceBus" &&
            subKey != "ParticularSoftware");
    }
}
