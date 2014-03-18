namespace PlatformInstaller
{
    using System.Windows.Input;

    public partial class ShellView
    {
        public ShellView()
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
