using System.Security.Principal;

public static class IsAdminChecker
{
    public static bool IsAdministrator()
    {
        using (var windowsIdentity = WindowsIdentity.GetCurrent())
        {
            var windowsPrincipal = new WindowsPrincipal(windowsIdentity);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}