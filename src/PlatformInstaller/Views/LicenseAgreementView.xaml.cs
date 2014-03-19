namespace PlatformInstaller
{

    public partial class LicenseAgreementView 
    {
        public LicenseAgreementView()
        {
            InitializeComponent();
            licenseBrowser.NavigateToString(LicenseText.ReadLicenseHtml());
        }
    }
}