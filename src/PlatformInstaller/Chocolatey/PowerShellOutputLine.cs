public class PowerShellOutputLine
{
    public PowerShellOutputLine(string text, PowerShellLineType type, bool newLine = true)
    {
        Text = text;
        Type = type;
        NewLine = newLine;
    }

    public string Text;
    public PowerShellLineType Type;
    public bool NewLine;
}