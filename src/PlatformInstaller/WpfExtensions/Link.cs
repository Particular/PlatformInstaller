using System;
using System.Diagnostics;
using System.Windows.Documents;

public class Link : Hyperlink
{
    public Link()
    {
        RequestNavigate += (sender, e) =>
        {
            try
            {
                using (Process.Start(new ProcessStartInfo(NavigateUri.PathAndQuery)))
                {
                }
            }
            catch{
                using (Process.Start(Uri.UnescapeDataString(NavigateUri.PathAndQuery)))
                {
                }
            }
        };
    }
}