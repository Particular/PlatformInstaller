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
                return completed.ToBytesString() + " of " + total.ToBytesString();
            }
        }
        return description;
    }
}