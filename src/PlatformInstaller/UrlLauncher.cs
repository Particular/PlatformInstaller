using System.Diagnostics;
using System.Windows;

public class UrlLauncher
{
    public static void Open(string uri)
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