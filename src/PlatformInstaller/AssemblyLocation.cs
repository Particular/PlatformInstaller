using System;
using System.IO;

public static class AssemblyLocation
{
    static AssemblyLocation()
    {
        var assembly = typeof(AssemblyLocation).Assembly;
        var uri = new UriBuilder(assembly.CodeBase);
        var currentAssemblyPath = Uri.UnescapeDataString(uri.Path);
        CurrentDirectory = Path.GetDirectoryName(currentAssemblyPath);
        ExeFileName = Path.GetFileNameWithoutExtension(currentAssemblyPath);
    }

    public static string ExeFileName;

    public static string CurrentDirectory;
}