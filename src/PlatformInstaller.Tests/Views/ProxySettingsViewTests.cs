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
        var releaseManager = new ReleaseManager(new FakeEventAggregator());
        return new ProxySettingsView(releaseManager)
                            {
                                Owner = ShellView.CurrentInstance
                            };
    }
}