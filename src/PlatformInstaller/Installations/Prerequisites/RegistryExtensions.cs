namespace PlatformInstaller.Installations.Prerequisites
{
    using System;
    using Microsoft.Win32;

    public static class RegistryExtensions
    {
        public static bool TrySetValue(this RegistryKey key, string name, object value, RegistryValueKind kind)
        {
            try
            {
                key.SetValue(name, value, kind);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
