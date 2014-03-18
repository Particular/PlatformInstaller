namespace PlatformInstaller
{
    using System.Windows.Input;

    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
