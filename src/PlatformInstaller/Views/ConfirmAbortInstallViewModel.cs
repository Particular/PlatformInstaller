using Caliburn.Micro;

public class ConfirmAbortInstallViewModel : Screen
{
    public ConfirmAbortInstallViewModel()
    {
        // ReSharper disable once DoNotCallOverridableMethodsInConstructor
        DisplayName = "Confirm Abort";
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