namespace PlatformInstaller
{
    using System.Windows;
    using Caliburn.Micro;

    public class InstallChocolateyViewModel : Screen
    {
        public void Continue()
        {
            UserChoseToContinue = true;
            base.TryClose();
        }

        public bool UserChoseToContinue;

        public void Cancel()
        {
           base.TryClose();
        }

        public void CopyCommand()
        {
            Clipboard.SetText(InstallCommand);
        }

        public string InstallCommand = "@powershell -NoProfile -ExecutionPolicy unrestricted -Command \"iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))\" && SET PATH=%PATH%;%systemdrive%\\chocolatey\\bin";
    }
}