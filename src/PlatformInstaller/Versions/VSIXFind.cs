using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public class VSIXFind
{
    public static bool TryFindInstalledVersion(string product, string visualStudioVersion, out Version versionFound)
    {
        versionFound = null;
        var rootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\VisualStudio");
        string extensions;

        switch (visualStudioVersion)
        {
            case VisualStudioVersions.VS2012:
                extensions = Path.Combine(rootDirectory, @"11.0\Extensions");
                break;
            case VisualStudioVersions.VS2013:
                extensions = Path.Combine(rootDirectory, @"12.0\Extensions");
                break;
            default:
                return false;
        }

        if (!Directory.Exists(extensions))
        {
            return false;
        }
        foreach (var file in Directory.EnumerateFiles(extensions, "extension.vsixmanifest", SearchOption.AllDirectories))
        {
            var contents = File.ReadAllText(file);
            var doc = XDocument.Parse(contents);
            var name = doc.Descendants().FirstOrDefault(p => p.Name.LocalName == "Name");
            if ((name != null) && (name.Value.IndexOf(product, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                var version = doc.Descendants().FirstOrDefault(p => p.Name.LocalName == "Version");
                if (version != null)
                {
                    if (Version.TryParse(version.Value, out versionFound))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
