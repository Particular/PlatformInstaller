using System.Windows;
using Caliburn.Micro;
using Resourcer;

public class LicenseAgreementViewModel : Screen
{
    IEventAggregator eventAggregator;

    public LicenseAgreementViewModel()
    {
        
    }

    public string License
    {
        get
        {
            return Resource.AsString("PlatformInstaller.LicenseAgreement.md");
        }
    }

    public LicenseAgreementViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "License Agreement";
    }

    public void Agree()
    {
        eventAggregator.Publish<AgreedToLicenseEvent>();
    }

    public void Exit()
    {
        eventAggregator.Publish<ExitApplicationCommand>();
    }


    public void Copy()
    {
        Clipboard.SetText(License);
    }

}