using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

public class Link : Hyperlink
{
    public Link()
    {
        RequestNavigate += (sender, e) =>
        {
            OpenUri(NavigateUri.AbsoluteUri);
        };
    }

    static void OpenUri(string uri)
    {
        try
        {
            using (Process.Start(uri))
            {
            }
        }
        catch 
        {
            Clipboard.SetText(uri);
            var message = $"The url {uri} failed to open and has been copied to your clipboard.";
            MessageBox.Show(message, "Failed to open browser", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}