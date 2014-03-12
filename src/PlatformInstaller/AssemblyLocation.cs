using System;
using System.IO;

internal static class AssemblyLocation
{
    public static string CurrentDirectory
    {
        get
        {
            var assembly = typeof(AssemblyLocation).Assembly;
            var uri = new UriBuilder(assembly.CodeBase);
            var currentAssemblyPath = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(currentAssemblyPath);
        }
    }
}