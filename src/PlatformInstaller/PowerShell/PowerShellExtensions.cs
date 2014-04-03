using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

public static class PowerShellExtensions
{
    public static string ToExecutableString(this Command command)
    {
        var stringBuilder = new StringBuilder("powershell -executionpolicy unrestricted \"" +command.CommandText);
        foreach (var parameter in command.Parameters)
        {
            if (parameter.Value == null)
            {
                stringBuilder.AppendFormat(" -{0} ", parameter.Name);
            }
            else
            {
                if (parameter.Value is string)
                {
                    stringBuilder.AppendFormat(" -{0} \'{1}\'", parameter.Name, parameter.Value);
                    continue;
                }
                if (parameter.Value is bool)
                {
                    var paramValue = (bool) parameter.Value;
                    if (paramValue)
                    {
                        stringBuilder.AppendFormat(" -{0}", parameter.Name);
                    }
                    continue;
                }
                stringBuilder.AppendFormat(" -{0} {1}", parameter.Name, parameter.Value);
            }
        }
        stringBuilder.Append("\"");
        return stringBuilder.ToString();
    }
    public static string ToDownloadingString(this ProgressRecord progressRecord)
    {
        var description = progressRecord.StatusDescription;
        if (description.StartsWith("Saving ") && description.Contains(" of "))
        {
            var strings = description.Replace("Saving ","").Split(new[]{" of "},StringSplitOptions.RemoveEmptyEntries);
            if (strings.Length == 2)
            {
                int completed;
                if (!int.TryParse(strings[0], out completed))
                {
                    return description;
                }
                int total;
                if (!int.TryParse(strings[1], out total))
                {
                    return description;
                }
                return BytesToString(completed) + " of " + BytesToString(total);
            }
        }
        return description;
    }
    static String BytesToString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
        {
            return "0" + suf[0];
        }
        var bytes = Math.Abs(byteCount);
        var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        var num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num) + suf[place];
    }
}