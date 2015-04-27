namespace PlatformInstaller.Releases
{
    using System;
    

    public class Release
    {
        public string Tag;
        public string ReleaseNotesUrl;
        public DateTime Published;
        public Asset[] Assets;
    }
}