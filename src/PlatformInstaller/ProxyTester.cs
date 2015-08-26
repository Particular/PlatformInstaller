using System.Net;

public class ProxyTester
{
    public static bool ProxyTest(ICredentials credentials)
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