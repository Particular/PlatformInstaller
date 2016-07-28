using System;
using Microsoft.Win32;

public static class RegistryHelper
{
    public static RegistryKey OpenSubKeyEx(this RegistryKey targetRegistryKey, string subKey, bool writable)
    {
        RegistryKey registryKey;
        try
        {
            registryKey = targetRegistryKey.OpenSubKey(subKey, writable);
        }
        catch (Exception exception)
        {
            throw new Exception($"Could not OpenSubKey: {subKey}", exception);
        }
        if (registryKey == null)
        {
            throw new Exception($"OpenSubKey returned null: {subKey}");
        }
        return registryKey;
    }

    public static RegistryKey CreateSubKeyEx(this RegistryKey targetRegistryKey, string subKey)
    {
        RegistryKey registryKey;
        try
        {
            registryKey = targetRegistryKey.CreateSubKey(subKey);
        }
        catch (Exception exception)
        {
            throw new Exception($"Could not CreateSubKey: {subKey}", exception);
        }
        if (registryKey == null)
        {
            throw new Exception($"CreateSubKey returned null: {subKey}");
        }
        return registryKey;
    }
}