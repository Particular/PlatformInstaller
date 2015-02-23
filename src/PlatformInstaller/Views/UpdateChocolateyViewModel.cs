using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

public class UpdateChocolateyViewModel : Screen
{
    IEventAggregator eventAggregator;
    ChocolateyInstaller chocolateyInstaller;

    public Version MinimumChocolateyVersion { get; set; }
    public Version DetectedChocolateyVersion { get; set; }
    public string LastCheckedTime { get; set; }
    public bool CanAcceptInput { get; set; }
    public string InstallCommand { get; set; }
    public string UpdateCommand { get; set; }

    public UpdateChocolateyViewModel()
    {
        
    }
    public UpdateChocolateyViewModel(IEventAggregator eventAggregator, ChocolateyInstaller chocolateyInstaller)
    {
        DisplayName = "Update Chocolatey";
        UpdateCommand = ChocolateyInstaller.UpdateCommand;
        InstallCommand = ChocolateyInstaller.InstallCommand;
        this.eventAggregator = eventAggregator;
        this.chocolateyInstaller = chocolateyInstaller;
    }
    
    public void Cancel()
    {
        eventAggregator.Publish<NavigateHomeCommand>();
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
                eventAggregator.Publish<UserInstalledChocolateyEvent>();
            }
        }
        finally
        {
            CanAcceptInput = true;
        }
    }
}