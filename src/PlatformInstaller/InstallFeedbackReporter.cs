using System.Diagnostics;
using System.Linq;
using Anotar.Serilog;
using Caliburn.Micro;
using Microsoft.Win32;

public class InstallFeedbackReporter : IHandle<InstallSucceededEvent>
{
    bool isNewUserAtStartup;

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

        Process.Start(@"http://particular.net/thank-you-for-installing-the-particular-service-platform?new_user=" + isNewUserAtStartup.ToString().ToLower());

        RecordSuccessfullInstallationFeeback();
    }

    void RecordSuccessfullInstallationFeeback()
    {
        using (var regRoot = Registry.CurrentUser.CreateSubKey(@"Software\ParticularSoftware\PlatformInstaller\"))
        {
            regRoot.SetValue("InstallationFeedbackReported","true");
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

    public static bool CheckRegistryForExistingKeys(RegistryKey regRoot)
    {
        return regRoot.GetSubKeyNames().All(subKey =>
            subKey != "NServiceBus" &&
            subKey != "ParticularSoftware");
    }

}