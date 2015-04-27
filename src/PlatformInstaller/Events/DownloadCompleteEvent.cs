using System;
    
public class DownloadCompleteEvent
{
    public Exception Error;
    public bool Cancelled;
    public string Name;
    public string FileName;
}
