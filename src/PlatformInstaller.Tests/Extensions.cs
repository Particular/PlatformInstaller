using System;
using System.Text;
using System.Threading;

public static class Extensions
{
    public static void WaitForTimeout(this WaitHandle handle, TimeSpan timeout)
    {
        var signalReceived = handle.WaitOne(timeout);
        if (!signalReceived)
        {
            throw new Exception("Timeout while waiting on handle");
        }
    }
    
    public static string ReplaceCaseless(this string str, string oldValue, string newValue)
    {
        var sb = new StringBuilder();

        var previousIndex = 0;
        var index = str.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
        while (index != -1)
        {
            sb.Append(str.Substring(previousIndex, index - previousIndex));
            sb.Append(newValue);
            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
        }
        sb.Append(str.Substring(previousIndex));

        return sb.ToString();
    }
}