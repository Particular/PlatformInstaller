using System;

public static class ReadableBytesConverter
{

    public static string ToBytesString(this int byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
        {
            return "0" + suf[0];
        }
        var bytes = Math.Abs(byteCount);
        var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        var num = Math.Round(bytes / Math.Pow(1024, place), 1);
        var d = Math.Sign(byteCount) * num;
        return $"{d:0.0}{suf[place]}";
    }
}