using System.Linq;
using Microsoft.Win32;

public class NewUserDetecter
{
    public bool IsNewUser()
    {
        return CheckForSubKey(Registry.CurrentUser);
    }

    public static bool CheckForSubKey(RegistryKey currentUser)
    {
        return currentUser.GetSubKeyNames().All(subKey =>
            subKey != "NServiceBus" &&
            subKey != "ParticularSoftware");
    }
}