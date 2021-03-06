using Caliburn.Micro;
using Resourcer;

public class LicenseAgreementViewModel : Screen
{
    IEventAggregator eventAggregator;

    public LicenseAgreementViewModel()
    {

    }

    public string License => Resource.AsString("PlatformInstaller.License.License.rtf");

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
}