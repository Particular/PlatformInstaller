using System.Net;
using Anotar.Serilog;

public class ProxyTester
{
    CredentialStore credentialStore;

    public ProxyTester(CredentialStore credentialStore)
    {
        this.credentialStore =  credentialStore;
    }

    public bool AreCredentialsRequired()
    {
        credentialStore.RetrieveSavedCredentials();

        if (TestCredentials(credentialStore.Credentials))
        {
            return false;
        }
        
        LogTo.Warning(credentialStore.Credentials == null ? "Failed to connect to the internet using anonymous credentials" : "Failed to connect to the internet using stored credentials");

        if (TestCredentials(CredentialCache.DefaultCredentials))
        { 
            credentialStore.Credentials = CredentialCache.DefaultCredentials;
            LogTo.Information("Successfully connect to the internet using default credentials");
            return false;
        }

        if (TestCredentials(CredentialCache.DefaultNetworkCredentials))
        {
            credentialStore.Credentials = CredentialCache.DefaultNetworkCredentials;
            LogTo.Information("Successfully connect to the internet using default network credentials");
            return false;
        }
        return true;
    }

    public bool TestCredentials(ICredentials credentials)
    {
        using (var client = new WebClient())
        {
            client.UseDefaultCredentials = true;
            client.Proxy.Credentials = credentials;
            try
            {
                client.DownloadString("http://platformupdate.particular.net/");
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null && response.StatusCode == HttpStatusCode.ProxyAuthenticationRequired)
                    {
                        return false;
                    }
                }
                throw;
            }
        }
        return true;
    }
}
