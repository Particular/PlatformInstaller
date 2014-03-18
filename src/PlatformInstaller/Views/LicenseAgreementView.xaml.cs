namespace PlatformInstaller
{
    using Resourcer;

    public partial class LicenseAgreementView 
    {
        public LicenseAgreementView()
        {
            InitializeComponent();
            var asString = Resource.AsString("LicenseAgreement.html");
            licenseBrowser.NavigateToString(asString);
        }
    }
}
