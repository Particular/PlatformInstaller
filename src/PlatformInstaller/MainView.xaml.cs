namespace PlatformInstaller
{
    using System.Windows.Input;

    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
