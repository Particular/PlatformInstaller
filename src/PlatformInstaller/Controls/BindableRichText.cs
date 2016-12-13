using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

public class BindableRichText : RichTextBox
{
        
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(BindableRichText), new PropertyMetadata(OnTextChanged));

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var rtf = d as BindableRichText;
        if (rtf == null)
            return;
        Load(rtf.Text, rtf.Document);
    }
        
    private static void Load(string rtftext, FlowDocument inFlowDocument)
    {
        var range = new TextRange(inFlowDocument.ContentStart, inFlowDocument.ContentEnd);
        var byteArray = Encoding.UTF8.GetBytes(rtftext);
        using (var stream = new MemoryStream(byteArray))
        {
            range.Load(stream, DataFormats.Rtf);
        }
    }
}
