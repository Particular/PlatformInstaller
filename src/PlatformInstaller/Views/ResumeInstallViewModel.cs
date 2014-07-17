using Caliburn.Micro;

public class ResumeInstallViewModel : Screen
{
    public void ConfirmCancellation()
    {
        DisplayName = "Resume Installation";
        AbortInstallation = true;
        TryClose();
    }

    public bool AbortInstallation;

    public void ContinueInstallation()
    {
        TryClose();
    }

}