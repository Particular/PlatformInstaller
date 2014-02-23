using System.Windows;

namespace PlatformInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mainViewModel = new MainViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.InstallMsmq();
        }
    }
}
