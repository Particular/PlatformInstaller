using Caliburn.Micro;

public class ResumeInstallViewModel : Screen
{
    public ResumeInstallViewModel()
    {
        // ReSharper disable once DoNotCallOverridableMethodsInConstructor
        DisplayName = "Resume Installation";
    }

    public void ConfirmCancellation()
    {
        AbortInstallation = true;
        TryClose();
    }

    public bool AbortInstallation { get; set; }

    public void ContinueInstallation()
    {
        TryClose();
    }

}