using System.Net;
using System.Windows;
using System.Windows.Input;

public partial class ProxySettingsView
{
    CredentialStore credentialStore;
    ProxyTester proxyTester;

    public ProxySettingsView(ProxyTester proxyTester, CredentialStore credentialStore)
    {
        this.credentialStore = credentialStore;
        this.proxyTester = proxyTester;
        InitializeComponent();
    }
    
    public bool Cancelled { get; set; }

    void OkClick(object sender, RoutedEventArgs e)
    {
        var credentials = new NetworkCredential(username.Text, password.Password);
        if (proxyTester.TestCredentials(credentials))
        {
            credentialStore.Credentials = credentials;
            if (saveCredentials.IsChecked.GetValueOrDefault(false))
            {
                SavedCredentials.SaveCredentials(username.Text, password.SecurePassword);    
            }
            Close();
        }
        else
        {
            MessageBox.Show(this, "The supplied username and password were not accepted by the proxy server","Credentials failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    void CancelClick(object sender, RoutedEventArgs e)
    {
        Cancelled = true;
        Close();
    }
}
