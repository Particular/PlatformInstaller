namespace PlatformInstaller
{
    using System.Diagnostics;
    using System.Windows.Documents;

    public class Link : Hyperlink
    {
        public Link() 
        {
            RequestNavigate += (sender, e) =>
            {
                using (Process.Start(new ProcessStartInfo(NavigateUri.PathAndQuery)))
                {
                }
            };
        }
    }
}