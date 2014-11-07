using System.Windows;
using Caliburn.Micro;

public class InstallChocolateyViewModel : Screen
{
    IEventAggregator eventAggregator;
    ChocolateyInstaller chocolateyInstaller;
    public string InstallCommand = ChocolateyInstaller.InstallCommand;

    public InstallChocolateyViewModel(IEventAggregator eventAggregator, ChocolateyInstaller chocolateyInstaller)
    {
        DisplayName = "Install Chocolatey";
        this.eventAggregator = eventAggregator;
        this.chocolateyInstaller = chocolateyInstaller;
    }

    public void Continue()
    {
        eventAggregator.Publish<AgreedToInstallChocolateyEvent>();
    }

    public void Cancel()
    {
        eventAggregator.Publish<NavigateHomeCommand>();
    }

    public void ReCheck()
    {
        if (chocolateyInstaller.IsInstalled())
        {
            eventAggregator.Publish<UserInstalledChocolateyEvent>();
        }
    }

    public void Copy()
    {
        Clipboard.SetText(InstallCommand);
    }

}