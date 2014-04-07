using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

public class UpdateChocolateyViewModel : Screen
{
    IEventAggregator eventAggregator;
    ChocolateyInstaller chocolateyInstaller;
    public Version MinimumChocolateyVersion;
    public Version DetectedChocolateyVersion;
    public string LastCheckedTime;
    public bool CanAcceptInput;
    public string InstallCommand = ChocolateyInstaller.InstallCommand;
    public string UpdateCommand = ChocolateyInstaller.UpdateCommand;

    public UpdateChocolateyViewModel(IEventAggregator eventAggregator, ChocolateyInstaller chocolateyInstaller)
    {
        this.eventAggregator = eventAggregator;
        this.chocolateyInstaller = chocolateyInstaller;
    }
    
    public void Cancel()
    {
        eventAggregator.Publish<HomeEvent>();
    }

    public void Copy()
    {
        Clipboard.SetText(InstallCommand);
    }

    protected override async void OnActivate()
    {
        await RefreshDetectedVersion();
        MinimumChocolateyVersion = chocolateyInstaller.MinimumChocolateyVersion;
    }

    protected override void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        CanAcceptInput = true;
    }

    async Task RefreshDetectedVersion()
    {
        var detectedChocolateyVersion = await chocolateyInstaller.GetInstalledVersion();
        DetectedChocolateyVersion = detectedChocolateyVersion;
        LastCheckedTime = DateTime.Now.ToString("h:mm:ss tt");
    }

    public async void ReCheck()
    {
        try
        {
            CanAcceptInput = false;
            await RefreshDetectedVersion();
            var chocolateyUpgradeRequired = await chocolateyInstaller.ChocolateyUpgradeRequired();
            if (chocolateyInstaller.IsInstalled() && !chocolateyUpgradeRequired)
            {
                eventAggregator.Publish<UserInstalledChocolatey>();
            }
        }
        finally
        {
            CanAcceptInput = true;
        }
    }
}