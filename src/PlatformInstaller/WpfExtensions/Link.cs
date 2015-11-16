using System.Windows.Documents;

public class Link : Hyperlink
{
    public Link()
    {
        RequestNavigate += (sender, e) =>
        {
            UrlLauncher.Open(NavigateUri.AbsoluteUri);
        };
    }
}