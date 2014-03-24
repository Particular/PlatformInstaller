using System.IO;

public class FileEx
{
    public static void CopyToDirectory(string file, string targetDirectory)
    {
        File.Copy(file, Path.Combine(targetDirectory, Path.GetFileName(file)), true);
    }
}