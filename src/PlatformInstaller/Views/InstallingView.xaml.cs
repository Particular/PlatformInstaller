using System.Collections.Specialized;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

public partial class InstallingView
{
    bool hasUserSelectedText;
    
    public InstallingView()
    {
        InitializeComponent();
        DataContextChanged += InstallingView_DataContextChanged;
    }


    void InstallingView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        var dataContext = DataContext as InstallingViewModel;
        if (dataContext != null)
        {
            var outputText = dataContext.OutputText;
            outputText.CollectionChanged += outputText_CollectionChanged;
        }
    }

    void outputText_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (InstallerOutputEvent x in e.NewItems)
        {
            Dispatcher.Invoke(() =>
            {
                var run = new Run(x.Text);
                if (x.IsError)
                {
                    run.Foreground = Brushes.LightCoral;
                }
                consoleTextBox.Document.Blocks.Add(new Paragraph(run));
                if (!hasUserSelectedText)
                {
                    consoleTextBox.ScrollToEnd();
                }
            });
        }
    }

    void consoleTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        hasUserSelectedText = true;
    }
}