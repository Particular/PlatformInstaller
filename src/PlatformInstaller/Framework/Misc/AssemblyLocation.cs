using System;
using System.IO;

public static class AssemblyLocation
{
    static AssemblyLocation()
    {
        var assembly = typeof(AssemblyLocation).Assembly;
        var uri = new UriBuilder(assembly.CodeBase);
        ExeFilePath = Uri.UnescapeDataString(uri.Path).Replace('/','\\');
        CurrentDirectory = Path.GetDirectoryName(ExeFilePath);
        ExeFileName = Path.GetFileNameWithoutExtension(ExeFilePath);
    }

    public static string ExeFilePath;
    public static string ExeFileName;

    public static string CurrentDirectory;
}