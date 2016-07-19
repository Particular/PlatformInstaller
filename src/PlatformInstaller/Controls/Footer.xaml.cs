using System.Reflection;

public partial class Footer
{
    public Footer()
    {
        InitializeComponent();
        AppVersion.Text = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";
    }
}
