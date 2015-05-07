using System;
using System.Diagnostics;

[DebuggerDisplay("{Tag}")]
public class Release
{
    public string Tag;
    public string ReleaseNotesUrl;
    public DateTime Published;
    public Asset[] Assets;
}