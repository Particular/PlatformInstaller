using System.Security;
using Microsoft.Win32;
using NUnit.Framework;

[TestFixture]
public class SavedCredentialsTests
{
    [Test]
    public void ReadWrite()
    {
        string username;
        SecureString password;
        DeleteCredentials();
        Assert.IsFalse(SavedCredentials.TryRetrieveSavedCredentials(out username, out password));
        Assert.IsNull(username);
        Assert.IsNull(password);
        SavedCredentials.SaveCedentials("username", "password".ToSecureString());
        Assert.IsTrue(SavedCredentials.TryRetrieveSavedCredentials(out username, out password));
        Assert.AreEqual("username", username);
        Assert.AreEqual("password", password.ToOriginalString());
    }

    static void DeleteCredentials()
    {
        Registry.CurrentUser.DeleteSubKey(@"Software\Particular\PlatformInstaller\Credentials", false);
    }
}