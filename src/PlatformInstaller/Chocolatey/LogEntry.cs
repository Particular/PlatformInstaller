public class LogEntry
{
    public LogEntry(string text, LogEntryType type, bool newLine = true)
    {
        Text = text;
        Type = type;
        NewLine = newLine;
    }

    public string Text;
    public LogEntryType Type;
    public bool NewLine;
}