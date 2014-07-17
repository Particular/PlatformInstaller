using Caliburn.Micro;

public class ResumeInstallViewModel : Screen
{
    public ResumeInstallViewModel()
    {
        DisplayName = "Resume Installation";
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