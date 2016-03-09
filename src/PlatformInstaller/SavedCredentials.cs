using System.Security;
using System.Security.Cryptography;
using Microsoft.Win32;

public static class SavedCredentials
{
    public static void SaveCredentials(string userName, SecureString password)
    {
        using (var credRegKey = Registry.CurrentUser.CreateSubKey(@"Software\Particular\PlatformInstaller\Credentials"))
        {
            if (credRegKey == null)
            {
                return;
            }
            credRegKey.SetValue("username", userName);
            var protect = ProtectedData.Protect(password.ToOriginalString().GetBytes(), null, DataProtectionScope.CurrentUser);
            credRegKey.SetValue("password", protect);
        }
    }

    public static bool TryRetrieveSavedCredentials(out string username, out SecureString password)
    {
        using (var credRegKey = Registry.CurrentUser.CreateSubKey(@"Software\Particular\PlatformInstaller\Credentials"))
        {
            var encryptedPassword = (byte[]) credRegKey?.GetValue("password", null);
            if (encryptedPassword != null)
            {
                username = (string) credRegKey.GetValue("username", null);
                var unprotected = ProtectedData.Unprotect(encryptedPassword, null, DataProtectionScope.CurrentUser);
                password = unprotected.GetString().ToSecureString();
                return true;
            }
        }

        username = null;
        password = null;
        return false;
    }
}