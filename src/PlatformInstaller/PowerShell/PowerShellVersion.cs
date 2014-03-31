using Microsoft.Win32;

public class PowerShellVersion
{
    public void Initialise()
    {
        IsInstalled = false;
        using (var powershellRegKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Powershell"))
        {
            if (powershellRegKey == null)
            {
                return;
            }
            
            // Powershell is grouped by Engine version - there is a different key for each grouping
            // PS 1 & 2 use the subkey "1"
            // PS 3 & 4 use the subkey "3"
            // If v3 and above is installed you can have at least two engines installed so enumerate over them 
            foreach (var subKeyName in powershellRegKey.GetSubKeyNames())
            {
                using (var subkey = powershellRegKey.OpenSubKey(subKeyName))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if ((int) subkey.GetValue("Install", 0) <= 0)
                    {
                        continue;
                    }
                    IsInstalled = true;
                    // ReSharper disable once PossibleNullReferenceException
                    using (var engineKey = subkey.OpenSubKey("PowerShellEngine"))
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        var output = (string) engineKey.GetValue("PowerShellVersion");
                        Version = System.Version.Parse(output).Major;
                    }
                }
            }
        }
    }

    public bool IsInstalled;
    public int Version;
}