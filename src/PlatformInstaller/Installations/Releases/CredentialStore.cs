using System.Net;
using System.Security;
using Anotar.Serilog;

public class CredentialStore
{
    public ICredentials Credentials;

    public static void SaveCredentials(NetworkCredential credentials)
    {
        SavedCredentials.SaveCredentials(credentials.UserName, credentials.SecurePassword);
    }

    public void RetrieveSavedCredentials()
    {
        SecureString password;
        string username;
        if (SavedCredentials.TryRetrieveSavedCredentials(out username, out password))
        {
            Credentials = new NetworkCredential(username, password);
            LogTo.Information("Using stored proxy credentials");
            return;
        }

        LogTo.Information("No stored proxy credentials");
    }

}