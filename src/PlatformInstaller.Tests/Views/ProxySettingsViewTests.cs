using System.Threading;
using ApprovalUtilities.Wpf;
using NUnit.Framework;

[TestFixture]
public class ProxySettingsViewTests
{
    [Test]
    [Explicit]
    [Apartment(ApartmentState.STA)]
    public void Show()
    {
        var proxySettings = CreateDialog();
        proxySettings.ShowDialog();
    }

    [Test]
    [Apartment(ApartmentState.STA)]
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
        var proxyTester = new ProxyTester(credentialStore);
        return new ProxySettingsView(proxyTester, credentialStore)
                            {
                                Owner = ShellView.CurrentInstance
                            };
    }
}