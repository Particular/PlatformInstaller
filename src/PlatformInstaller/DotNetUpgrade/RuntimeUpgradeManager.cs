using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Serilog;
using Caliburn.Micro;
using Microsoft.Win32;

public class RuntimeUpgradeManager
{
    IEventAggregator eventAggregator;
    CredentialStore credentialStore;
    ProcessRunner processRunner;
    const string dotNet452WebInstallerExe = "NDP452-KB2901954-Web.exe";

    // This download link came from .NET Framework Deployment Guide for Developers -  see https://msdn.microsoft.com/en-us/library/ee942965%28v=vs.110%29.aspx#redist
    const string dotNet452WebInstallerURL = "http://go.microsoft.com/fwlink/?LinkId=397707";
    FileInfo installer;

    public RuntimeUpgradeManager(IEventAggregator eventAggregator, CredentialStore credentialStore, ProcessRunner processRunner)
    {
        this.eventAggregator = eventAggregator;
        this.credentialStore = credentialStore;
        this.processRunner = processRunner;

        var tempFolder = new DirectoryInfo(Environment.ExpandEnvironmentVariables("%temp%"));
        var assetFolder = Directory.CreateDirectory(Path.Combine(tempFolder.FullName, @"Particular\PlatformInstaller"));
        installer = new FileInfo(Path.Combine(assetFolder.FullName, dotNet452WebInstallerExe));
    }
    
    public bool Is452InstallRequired()
    {
        using (var basekey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
        using (var ndpKey = basekey.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
        {
            if (ndpKey?.GetValueKind("Release") == RegistryValueKind.DWord)
            {
                var release = (int) ndpKey.GetValue("Release");
                return release < 379893;
            }
        }
        return false;
    }

    public async Task<FileInfo> Download452WebInstaller()
    {
        eventAggregator.PublishOnUIThread(new DotNetDownloadStartedEvent());
        using (var client = new WebClient())
        {
            client.Proxy.Credentials = credentialStore.Credentials;
            client.DownloadProgressChanged += OnClientOnDownloadProgressChanged;
            client.DownloadFileCompleted += OnClientOnDownloadFileCompleted;

            var retries = 0;
            const int maxretries = 5;

            while (true)
            {
                LogTo.Information("Attempting to download '{0}' to '{1}'", dotNet452WebInstallerURL, installer.FullName);
                try
                {
                    await client.DownloadFileTaskAsync(dotNet452WebInstallerURL, installer.FullName).ConfigureAwait(false);
                    return installer;
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
                }
            }
        }
    }

    public async void InstallDotNet452()
    {

        var exitCode = await processRunner.RunProcess(installer.FullName,
            "/passive",
            // ReSharper disable once PossibleNullReferenceException
            installer.Directory.FullName,
            s => { },
            s => { }).ConfigureAwait(false);
        
        if (exitCode == 0)
        {
            eventAggregator.PublishOnUIThread(new DotNetInstallCompleteEvent());
        }
        else
        {
            eventAggregator.PublishOnUIThread(new DotNetInstallFailedEvent { ExitCode = exitCode });
        }
    }
    
    void PublishFailed()
    {
        eventAggregator.PublishOnUIThread(new DotNetDownloadFailedEvent
        {
            Text = "Download failed. Retrying..."
        });
    }

    void PublishAborted()
    {
        eventAggregator.PublishOnUIThread(new DotNetDownloadAbortedEvent
        {
            IsError = true,
            Text = "Download did not complete. Installation Aborted"
        });
    }

    void OnClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs args)
    {
        eventAggregator.PublishOnUIThread(new DotNetDownloadCompleteEvent());
    }

    void OnClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs args)
    {
        eventAggregator.PublishOnUIThread(new DotNetDownloadProgressEvent
        {
            BytesReceived = args.BytesReceived,
            ProgressPercentage = args.ProgressPercentage,
            TotalBytes = args.TotalBytesToReceive
        });
    }
}