using ApprovalUtilities.Wpf;
using NUnit.Framework;

[TestFixture]
public class ProxySettingsViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void Show()
    {
        var proxySettings = CreateDialog();
        proxySettings.ShowDialog();
    }
    
    [Test]
    [RequiresSTA]
    [Explicit]
    public void ScreenShot()
    {
        var proxySettings = CreateDialog();
        proxySettings.Show();
        var filename = "ProxySettingsView.png";
        WpfUtils.ScreenCapture(proxySettings, filename);
    }

    static ProxySettingsView CreateDialog()
    {
        var credentialStore = new CredentialStore();
        return new ProxySettingsView(credentialStore)
                            {
                                Owner = ShellView.CurrentInstance
                            };
    }
}