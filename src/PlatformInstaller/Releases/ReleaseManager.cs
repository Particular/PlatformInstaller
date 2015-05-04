using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Win32;
using Newtonsoft.Json;

public class ReleaseManager
{
    IEventAggregator eventAggregator;
    public ReleaseManager(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
       
    }

    public ICredentials Credentials;

    const string rootURL = @"http://platformupdate.particular.net";

    public static bool ProxyTest(ICredentials credentials)
    {
        using (var client = new WebClient())
        {
            try
            {
               client.Proxy.Credentials = credentials;
               client.DownloadString(rootURL);
            }
            catch(WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null && response.StatusCode == HttpStatusCode.ProxyAuthenticationRequired)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public Release[] GetReleasesForProduct(string product)
    {
        var uri = string.Format("{0}/{1}.txt", rootURL, product).ToLower();
        using (var client = new WebClient())
        {
            client.Proxy.Credentials = Credentials;

            string data = null;
            var retries = 0;
            const int maxretries = 5;
            while (true)
            {
                try
                {
                    data = client.DownloadString(uri);
                    break;
                }
                catch
                {
                    retries++;
                    Thread.Sleep(250);
                    if (retries > maxretries)
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(data))
            {
                return (Release[])JsonConvert.DeserializeObject(data, typeof(Release[]));    
            }
            return new Release[] {};
        }
    }

    public IEnumerable<FileInfo> DownloadRelease(Release release, string filter = null)
    {
        var tempFolder = new DirectoryInfo(Environment.ExpandEnvironmentVariables("%temp%"));

        var assetFolder = Directory.CreateDirectory(Path.Combine(tempFolder.FullName, @"Particular\PlatformInstaller"));

        using (var client = new WebClient())
        {
            client.Proxy.Credentials = Credentials;

            var assets = (filter == null) ? release.Assets : release.Assets.Where(p => p.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1).ToArray();
            foreach (var asset in assets)
            {
                var localAsset = new FileInfo(Path.Combine(assetFolder.FullName, asset.Name));
                if (!localAsset.Exists || localAsset.Length != asset.Size)
                {
                    eventAggregator.PublishOnUIThread(new DownloadStartedEvent
                    {
                        Url = asset.Name,
                        FileName = localAsset.FullName
                    });
                        
                    client.DownloadProgressChanged += (sender, args) => {
                        eventAggregator.PublishOnUIThread(new DownloadProgressEvent{
                            BytesReceived = args.BytesReceived,
                            ProgressPercentage = args.ProgressPercentage,
                            TotalBytes = args.TotalBytesToReceive
                        });
                    };
                    client.DownloadFileCompleted += (sender, args) =>
                    {
                        eventAggregator.PublishOnUIThread(new DownloadCompleteEvent());
                    };

                    var retries = 0;
                    const int maxretries = 5;
                    while (true)
                    {
                        var t = client.DownloadFileTaskAsync(asset.Download, localAsset.FullName);
                        try
                        {
                            Task.WaitAll(t);
                            break;
                        }
                        catch 
                        {
                            retries++;
                            eventAggregator.PublishOnUIThread(new InstallerOutputEvent{Text = "Download failed. Retrying..."});
                            Thread.Sleep(500);
                            if (retries > maxretries)
                            {
                                eventAggregator.PublishOnUIThread(new InstallerOutputEvent
                                {
                                    IsError = true,
                                    Text = "Download did not complete. Installation Aborted"
                                });
                                throw new Exception("Download did not complete");
                            }
                        }
                    }

                }
                yield return localAsset;
            }
        }
    }

    public static void SaveCredentials(NetworkCredential credentials)
    {
        using (var credRegKey = Registry.CurrentUser.CreateSubKey(@"Software\Particular\PlatformInstaller\Credentials"))
        {
            if (credRegKey == null)
            {
                return;
            }
            credRegKey.SetValue("username", credentials.UserName);
            credRegKey.SetValue("password", ProtectedData.Protect(credentials.Password.GetBytes(), null, DataProtectionScope.CurrentUser));
        }
    }

    public void RetrieveSavedCredentials()
    {
        using (var credRegKey = Registry.CurrentUser.CreateSubKey(@"Software\Particular\PlatformInstaller\Credentials"))
        {
            if (credRegKey == null)
            {
                return;
            }
            var username = (string) credRegKey.GetValue("username", null);
            var encryptedPassword = (byte[]) credRegKey.GetValue("password", null);

            if (encryptedPassword != null)
            {
                Credentials = new NetworkCredential(username, ProtectedData.Unprotect(encryptedPassword, null, DataProtectionScope.CurrentUser).GetString());
            }
        }
    }
}
