namespace PlatformInstaller
{
    using Caliburn.Micro;

    public class LicenseAgreementViewModel: Screen
    {
        IEventAggregator eventAggregator;

        public LicenseAgreementViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Agree()
        {
            eventAggregator.Publish<AgeedToLicenseEvent>();
        }

        public void Close()
        {
            eventAggregator.Publish<CloseApplicationEvent>();
        }

    }
}