public static class ResourceResolver
{
    public static string GetPackUrl(string image)
    {
        return $"pack://application:,,,/PlatformInstaller;component{image}";
    }
}