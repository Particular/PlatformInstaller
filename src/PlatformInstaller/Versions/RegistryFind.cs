namespace PlatformInstaller.Versions
{
    using System;
    using Microsoft.Win32;
    using NuGet;

    public class RegistryFind
    {
        public static bool TryFindInstalledVersion(string product, out SemanticVersion versionFound)
        {
            const string uninstallKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";

            var views = new[]
            {
                RegistryView.Registry32,
                RegistryView.Registry64
            };

            foreach (var view in views)
            {
                using (var localMachineRegistry = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                using (var key = localMachineRegistry.OpenSubKey(uninstallKeyPath))
                {
                    if (key == null)
                    {
                        continue;
                    }
                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        using (var subKey = key.OpenSubKey(subKeyName))
                        {
                            if (subKey != null)
                            {
                                var publisher = (string) subKey.GetValue("Publisher", string.Empty, RegistryValueOptions.None);
                           
                                if (publisher.StartsWith("Particular Software", StringComparison.OrdinalIgnoreCase))
                                {  
                                    var installDate = (string) subKey.GetValue("InstallDate", null, RegistryValueOptions.None);
                                    var productName = (string) subKey.GetValue("DisplayName",string.Empty, RegistryValueOptions.None);

                                    if ((installDate != null) && (productName.IndexOf(product, StringComparison.OrdinalIgnoreCase) >= 0))
                                    {
                                        var version = (string) subKey.GetValue("DisplayVersion",string.Empty, RegistryValueOptions.None);
                                        SemanticVersion.TryParse(version, out versionFound);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            versionFound = null;
            return false;
        }
    }
}
