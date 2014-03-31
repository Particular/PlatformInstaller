using System.Windows.Controls;

public static class WpfExtensions
{
    public static void CopyToClipboard(this RichTextBox richTextBox)
    {
        var textSelection = richTextBox.Selection;
        var start = textSelection.Start;
        var textPointer = textSelection.End;
        richTextBox.SelectAll();
        richTextBox.Copy();
        richTextBox.Selection.Select(start, textPointer);
    }
}