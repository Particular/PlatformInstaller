using System;
using System.Collections.Generic;
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

    public IEnumerable<FileInfo> DownloadRelease(Release release, string filter = null)
    {
        var tempFolder = new DirectoryInfo(Environment.ExpandEnvironmentVariables("%temp%"));

        var assetFolder = Directory.CreateDirectory(Path.Combine(tempFolder.FullName, @"Particular\PlatformInstaller"));

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

                Action<DownloadProgressInfo> progressAction = args =>
                {
                    eventAggregator.PublishOnUIThread(new DownloadProgressEvent
                    {
                        BytesReceived = args.BytesReceived,
                        ProgressPercentage = args.ProgressPercentage,
                        TotalBytes = args.TotalBytesToReceive
                    });
                };

                var retries = 0;
                const int maxretries = 5;
                while (true)
                {
                    try
                    {
                        DownloadToFile(asset.Download, localAsset.FullName, progressAction).GetAwaiter().GetResult();
                        break;
                    }
                    catch (Exception ex)
                    {
                        retries++;
                        eventAggregator.PublishOnUIThread(new InstallerOutputEvent { Text = "Download failed. Retrying..." });
                        Thread.Sleep(500);
                        if (retries > maxretries)
                        {
                            eventAggregator.PublishOnUIThread(new InstallerOutputEvent
                            {
                                IsError = true,
                                Text = "Download did not complete. Installation Aborted"
                            });
                            throw new Exception("Download did not complete", ex);
                        }
                    }
                }

                eventAggregator.PublishOnUIThread(new DownloadCompleteEvent());
            }
            yield return localAsset;
        }
    }


    private async Task DownloadToFile(string address, string filename, Action<DownloadProgressInfo> progress)
    {
        if (File.Exists(filename))
        {
            return;
        }

        var fileInfo = new FileInfo(filename + ".part");

        var request = WebRequest.CreateHttp(address);
        request.Proxy.Credentials = credentialStore.Credentials;
        if (fileInfo.Exists)
        {
            request.AddRange(fileInfo.Length);
        }

        using (var response = (HttpWebResponse)(await request.GetResponseAsync().CatchWebException().ConfigureAwait(false)))
        {
            FileStream saveStream = null;
            long currentLength = 0;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                saveStream = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }
            else if (response.StatusCode == HttpStatusCode.PartialContent)
            {
                saveStream = new FileStream(fileInfo.FullName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                currentLength = fileInfo.Length;
            }

            if (saveStream == null)
            {
                throw new Exception("Download failed: " + response.StatusDescription);
            }

            using (saveStream)
            {
                await Task.Factory.Iterate(CopyStreamIterator(response.GetResponseStream(), saveStream, currentLength, response.ContentLength, progress)).ConfigureAwait(false);
            }

            if (fileInfo.Length != response.ContentLength)
            {
                throw new Exception("Download stream closed unexpectedly.");
            }
        }

        fileInfo.MoveTo(filename);
    }

    private static IEnumerable<Task> CopyStreamIterator(Stream input, Stream output, long partialBytes, long lengthBytes, Action<DownloadProgressInfo> progress)
    {
        // Create two buffers.  One will be used for the current read operation and one for the current
        // write operation.  We'll continually swap back and forth between them.
        var buffers = new[] { new byte[BUFFER_SIZE], new byte[BUFFER_SIZE] };
        var filledBufferNum = 0;
        Task writeTask = null;

        var totalBytes = partialBytes + lengthBytes;
        var currentBytes = partialBytes;

        // Until there's no more data to be read
        while (true)
        {
            // Read from the input asynchronously
            var readTask = input.ReadAsync(buffers[filledBufferNum], 0, buffers[filledBufferNum].Length).CatchIOException(0);

            // If we have no pending write operations, just yield until the read operation has
            // completed.  If we have both a pending read and a pending write, yield until both the read
            // and the write have completed.
            if (writeTask == null)
            {
                yield return readTask;
                readTask.Wait(); // propagate any exception that may have occurred
            }
            else
            {
                var tasks = new[] { readTask, writeTask };
                yield return Task.Factory.WhenAll(tasks);
                Task.WaitAll(tasks); // propagate any exceptions that may have occurred
            }

            currentBytes += readTask.Result;

            progress(new DownloadProgressInfo { BytesReceived = currentBytes, TotalBytesToReceive = totalBytes });

            // If no data was read, nothing more to do.
            if (readTask.Result <= 0) break;

            // Otherwise, write the written data out to the file
            writeTask = output.WriteAsync(buffers[filledBufferNum], 0, readTask.Result);

            // Swap buffers
            filledBufferNum ^= 1;
        }
    }

    private class DownloadProgressInfo
    {
        public int ProgressPercentage { get { return (int)(BytesReceived * 100 / TotalBytesToReceive); } }

        public long BytesReceived { get; set; }

        public long TotalBytesToReceive { get; set; }
    }
}