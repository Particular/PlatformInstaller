using System.Diagnostics;
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
        RunUrlAndRecordFeedback(@"http://particular.net/thank-you-for-installing-the-particular-service-platform?new_user={0}&installed={1}&nuget={2}", isNewUserAtStartup.ToString().ToLower(), string.Join(";", message.InstalledItems),  NugetFlag());
    }

    public void Handle(InstallCancelledEvent message)
    {
        // Show the feedback page on every cancelled install
        RunUrlAndRecordFeedback(cancelledUrl);
    }

    public void Handle(NoInstallAttemptedEvent message)
    {
        // Show the feedback page on first run if they close without installing stuff
        if (HasFeebackBeenReportedForThisMachine())
            return;
        RunUrlAndRecordFeedback(cancelledUrl);
    }

    void RunUrlAndRecordFeedback(string url, params object[] args)
    {
        Process.Start(string.Format(url, args));
        RecordInstallationFeeback();
    }

    void RecordInstallationFeeback()
    {
        using (var regRoot = Registry.CurrentUser.CreateSubKey(@"Software\ParticularSoftware\PlatformInstaller\"))
        {
            regRoot.SetValue("InstallationFeedbackReported", "true");
        }
    }

    bool HasFeebackBeenReportedForThisMachine()
    {
        using (var regRoot = Registry.CurrentUser.OpenSubKey(@"Software\ParticularSoftware\PlatformInstaller\"))
        {
            return regRoot != null && regRoot.GetValue("InstallationFeedbackReported") != null;
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
                return (string) (regRoot.GetValue("NuGetUser", "false"));
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
