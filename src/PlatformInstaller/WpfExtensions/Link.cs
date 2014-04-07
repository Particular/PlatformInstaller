using System.Diagnostics;
using System.Windows.Documents;

public class Link : Hyperlink
{
    public Link()
    {
        RequestNavigate += (sender, e) =>
        {
            using (Process.Start(NavigateUri.PathAndQuery))
            {
            }
        };
    }
}