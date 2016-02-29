using Microsoft.Win32;

public class DotnetVersionChecker
{
    private static bool Is452orLaterInstalled()
    {
        using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
        {
            if (ndpKey != null && ndpKey.GetValue("Release") != null)
            {
                int release;
                if (int.TryParse((string)ndpKey.GetValue("Release"), out release))
                {
                    return release >= 379893;
                }
            }
        }
        return false;
    }
}
