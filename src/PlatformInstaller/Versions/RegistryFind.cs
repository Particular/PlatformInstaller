using System;
using System.Collections.Generic;
using Microsoft.Win32;

public class RegistryFind
{
    public static bool TryFindInstalledVersion(string productName, out Version versionFound)
    {
        foreach (var product in FindInstalledProducts())
        {
            if (!product.Publisher.StartsWith("Particular Software"))
            {
                continue;
            }
            if (!product.ProductName.Contains(productName))
            {
                continue;
            }
            Version.TryParse(product.Version, out versionFound);
            return true;
        }
        versionFound = null;
        return false;
    }

    public static IEnumerable<InstalledProduct> FindInstalledProducts()
    {
        var uninstallKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
        foreach (var view in GetViewsToProcess())
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
                        var installDate = (string) subKey?.GetValue("InstallDate", null, RegistryValueOptions.None);
                        if (installDate == null)
                        {
                            // there are duplicates under entries in Software\Microsoft\Windows\CurrentVersion\Uninstall
                            // we only want the verbose entry and we use the side effect that the installDate contains
                            // data in the verbose version
                            continue;
                        }
                        yield return new InstalledProduct
                        {
                            ProductName = (string) subKey.GetValue("DisplayName", string.Empty, RegistryValueOptions.None),
                            Version = (string) subKey.GetValue("DisplayVersion", string.Empty, RegistryValueOptions.None),
                            Publisher = (string) subKey.GetValue("Publisher", string.Empty, RegistryValueOptions.None)
                        };
                    }
                }
            }
        }
    }

    static IEnumerable<RegistryView> GetViewsToProcess()
    {
        if (Environment.Is64BitOperatingSystem)
        {
            yield return RegistryView.Registry64;
        }
        yield return RegistryView.Registry32;
    }
}