using Caliburn.Micro;

public class ConfirmAbortInstallViewModel : Screen
{
    public ConfirmAbortInstallViewModel()
    {
        DisplayName = "Confirm Abort";
    }
    public void ConfirmCancellation()
    {
        AbortInstallation = true;
        TryClose();
    }

    public bool AbortInstallation;

    public void ContinueInstallation()
    {
        TryClose();
    }

}