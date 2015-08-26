using System.Net;
using System.Windows;
using System.Windows.Input;

public partial class ProxySettingsView
{
    CredentialStore credentialStore;

    public ProxySettingsView(CredentialStore credentialStore)
    {
        this.credentialStore = credentialStore;
        InitializeComponent();
    }

    
    public bool Cancelled { get; set; }

    void OkClick(object sender, RoutedEventArgs e)
    {
        var credentials = new NetworkCredential(username.Text, password.Password);
        if (ProxyTester.ProxyTest(credentials))
        {
            credentialStore.Credentials = credentials;
            if (saveCredentials.IsChecked.GetValueOrDefault(false))
            {
                SavedCredentials.SaveCedentials(username.Text, password.SecurePassword);    
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
