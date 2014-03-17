using System.Linq;
using Microsoft.Win32;

public class NewUserDetecter
{
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