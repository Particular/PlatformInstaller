using System.Net;
using System.Windows;
using System.Windows.Input;

public partial class ProxySettingsView
{
    ReleaseManager releaseManager;   

    public ProxySettingsView(ReleaseManager releaseManager)
    {
        this.releaseManager = releaseManager;
        InitializeComponent();
    }

    
    public bool Cancelled { get; set; }

    private void OkClick(object sender, RoutedEventArgs e)
    {
        var credentials = new NetworkCredential(username.Text, password.Password);
        if (ReleaseManager.ProxyTest(credentials))
        {
            releaseManager.Credentials = credentials;
            if (saveCredentials.IsChecked.GetValueOrDefault(false))
            {
                ReleaseManager.SaveCredentials(credentials);    
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
