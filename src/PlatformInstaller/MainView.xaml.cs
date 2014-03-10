
namespace PlatformInstaller
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}
