using Caliburn.Micro;

public class AcceptWarningsViewModel : Screen
{
    public bool AbortInstallation { get; set; }

    public string[] Problems { get; set; }

    public void ContinueInstallation()
    {
        TryClose();
    }

    public void ConfirmCancellation()
    {
        AbortInstallation = true;
        TryClose();
    }
}