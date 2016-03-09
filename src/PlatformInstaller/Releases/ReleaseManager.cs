using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Serilog;
using Caliburn.Micro;
using Newtonsoft.Json;

public class ReleaseManager
{
    IEventAggregator eventAggregator;
    CredentialStore credentialStore;

    public ReleaseManager(IEventAggregator eventAggregator, CredentialStore credentialStore)
    {
        this.eventAggregator = eventAggregator;
        this.credentialStore = credentialStore;
    }
   
    const string rootURL = @"http://platformupdate.particular.net/{0}.txt";

    public Release[] GetReleasesForProduct(string product)
    {
        var uri = string.Format(rootURL, product.ToLowerInvariant());
        using (var client = new WebClient())
        {
            client.Proxy.Credentials = credentialStore.Credentials;

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
                    {
                        break;
                    }
                    LogTo.Information("Retrying to retrieve data from {0}", uri);
                }
            }

            if (!string.IsNullOrWhiteSpace(data))
            {
                var releases = (Release[])JsonConvert.DeserializeObject(data, typeof(Release[]));
                if (releases.Any())
                {
                    return releases;
                }
            }
            throw new Exception("Could not load releases from " + uri);
        }
    }

    public async Task<FileInfo> DownloadRelease(Asset release)
    {
        var tempFolder = new DirectoryInfo(Environment.ExpandEnvironmentVariables("%temp%"));

        var assetFolder = Directory.CreateDirectory(Path.Combine(tempFolder.FullName, @"Particular\PlatformInstaller"));
        var localAsset = new FileInfo(Path.Combine(assetFolder.FullName, release.Name));
        if (localAsset.Exists && localAsset.Length == release.Size)
        {
            return localAsset;
        }
        PublishStart(release, localAsset);
        using (var client = new WebClient())
        {
            client.Proxy.Credentials = credentialStore.Credentials;
            client.DownloadProgressChanged += OnClientOnDownloadProgressChanged;
            client.DownloadFileCompleted += OnClientOnDownloadFileCompleted;

            var retries = 0;
            const int maxretries = 5;
            while (true)
            {
                var url = release.Download;
                var fullName = localAsset.FullName;
                LogTo.Information("Attempting to download '{0}' to '{1}'", url, fullName);
                try
                {
                    await client.DownloadFileTaskAsync(url, fullName).ConfigureAwait(false);
                    return localAsset;
                }
                catch
                {
                    retries++;
                    PublishFailed();
                    await Task.Delay(500).ConfigureAwait(false);
                    if (retries <= maxretries)
                    {
                        continue;
                    }
                    PublishAborted();
                    break;
                }
            }
        }
        return null;
    }

    void PublishStart(Asset release, FileInfo localAsset)
    {
        eventAggregator.PublishOnUIThread(new DownloadStartedEvent
        {
            Url = release.Name,
            FileName = localAsset.FullName
        });
    }

    void PublishFailed()
    {
        eventAggregator.PublishOnUIThread(new InstallerOutputEvent
        {
            Text = "Download failed. Retrying..."
        });
    }

    void PublishAborted()
    {
        eventAggregator.PublishOnUIThread(new InstallerOutputEvent
        {
            IsError = true,
            Text = "Download did not complete. Installation Aborted"
        });
    }

    void OnClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs args)
    {
        eventAggregator.PublishOnUIThread(new DownloadCompleteEvent());
    }

    void OnClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs args)
    {
        eventAggregator.PublishOnUIThread(new DownloadProgressEvent
        {
            BytesReceived = args.BytesReceived,
            ProgressPercentage = args.ProgressPercentage,
            TotalBytes = args.TotalBytesToReceive
        });
    }
}