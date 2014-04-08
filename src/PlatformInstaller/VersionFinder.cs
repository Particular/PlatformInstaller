using System;
using System.Reflection;

public class VersionFinder
{
    public static string GetVersion()
    {
        var type = typeof(VersionFinder);

        var customAttributes = type.Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);

        if (customAttributes.Length >= 1)
        {
            var fileVersion = (AssemblyInformationalVersionAttribute)customAttributes[0];
            return fileVersion.InformationalVersion;
        }

        return type.Assembly.GetName().Version.ToString(3);
    }
}