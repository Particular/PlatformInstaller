using Microsoft.Win32;

public class LicenseAgreement 
{
    public bool HasAgreedToLicense()
    {
        using (var registryKey = GetPlatformInstallerRegistryKey())
        {
            var value = registryKey.GetValue("AgreedToLicense");
            if (value == null)
            {
                return false;
            }
            return ((string) value) == "True";
        }
    }

    static RegistryKey GetPlatformInstallerRegistryKey()
    {
        return Registry.CurrentUser.CreateSubKey(@"Software\ParticularSoftware\PlatformInstaller\");
    }

    public void Agree()
    {
        using (var registryKey = GetPlatformInstallerRegistryKey())
        {
            registryKey.SetValue("AgreedToLicense", true);
        }
    }

    public void Clear()
    {
        using (var registryKey = GetPlatformInstallerRegistryKey())
        {
            registryKey.DeleteValue("AgreedToLicense", false);
        }
    }
}