using System.Diagnostics;
using System.Linq;
using Anotar.Serilog;
using Caliburn.Micro;
using Microsoft.Win32;

public class NewUserDetecter : IHandle<InstallSucceededEvent>
{
    bool isNewUserAtStartup;
    bool hasProcessedSucceed;

    public NewUserDetecter()
    {
        isNewUserAtStartup = IsNewUser();
    }

    public void Handle(InstallSucceededEvent message)
    {
        if (hasProcessedSucceed)
        {
            LogTo.Information("Install feedback has already been reported no new browser will be popped");
            return;
        }


        LogTo.Information("Install successfull, new user: " + isNewUserAtStartup);

        Process.Start(@"http://particular.net/thank-you-for-downloading-the-particular-service-platform?new_user=" + isNewUserAtStartup.ToString().ToLower());
        hasProcessedSucceed = true;
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