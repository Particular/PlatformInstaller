using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Serilog;
using Caliburn.Micro;
using Newtonsoft.Json;

public class ReleaseManager
{
    const int BUFFER_SIZE = 0x1000;

    IEventAggregator eventAggregator;
    CredentialStore credentialStore;

    public ReleaseManager(IEventAggregator eventAggregator, CredentialStore credentialStore)
    {
        this.eventAggregator = eventAggregator;
        this.credentialStore = credentialStore;
    }

    public List<string> FailedFeeds = new List<string>();
    
    const string rootURL = @"http://platformupdate.particular.net/{0}.txt";


    public Release[] GetReleasesForProduct(string product)
    {
        var uri = string.Format(rootURL, product).ToLower();
        return DownloadReleases(uri);
    }

    private Release[] DownloadReleases(string uri)
    {
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
                return (Release[])JsonConvert.DeserializeObject(data, typeof(Release[]));
            }
            lock (FailedFeeds)
            {
                if (!FailedFeeds.Contains(uri))
                {
                    LogTo.Error("Failed to retrieve data from {0}", uri);
                    FailedFeeds.Add(uri);
                }
            }
            return new Release[] { };
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
                try
                {
                 await client.DownloadFileTaskAsync(url, fullName).ConfigureAwait(false);
                    return localAsset;
                }
                catch
                {
                    retries++;
                    PublishFailed();
                    Thread.Sleep(500);
                    if (retries <= maxretries)
                    {
                        continue;
                    }
                    PublishAborted();
                    throw new Exception("Download did not complete");
                }
            }
        }
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