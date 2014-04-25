using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

//example  <global:GifImage x:Name="gifImage" Stretch="None" GifSource="pack://application:,,,/PlatformInstaller;component/Images/Spinner.gif" AutoStart="True" />
class GifImage : Image
{
    bool isInitialized;
    GifBitmapDecoder gifDecoder;
    Int32Animation animation;

    public int FrameIndex
    {
        get { return (int) GetValue(FrameIndexProperty); }
        set { SetValue(FrameIndexProperty, value); }
    }

    void Initialize()
    {
        gifDecoder = new GifBitmapDecoder(new Uri(GifSource), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
        animation = new Int32Animation(0, gifDecoder.Frames.Count - 1, new Duration(new TimeSpan(0, 0, 0, gifDecoder.Frames.Count/10, (int) ((gifDecoder.Frames.Count/10.0 - gifDecoder.Frames.Count/10)*1000))))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
        Source = gifDecoder.Frames[0];

        isInitialized = true;
    }

    static GifImage()
    {
        VisibilityProperty.OverrideMetadata(typeof(GifImage), new FrameworkPropertyMetadata(VisibilityPropertyChanged));
    }

    static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if ((Visibility) e.NewValue == Visibility.Visible)
        {
            ((GifImage) sender).StartAnimation();
        }
        else
        {
            ((GifImage) sender).StopAnimation();
        }
    }

    public static DependencyProperty FrameIndexProperty =
        DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage), new UIPropertyMetadata(0, ChangingFrameIndex));

    static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
    {
        var gifImage = (GifImage) obj;
        gifImage.Source = gifImage.gifDecoder.Frames[(int) ev.NewValue];
    }

    public bool AutoStart
    {
        get { return (bool) GetValue(AutoStartProperty); }
        set { SetValue(AutoStartProperty, value); }
    }

    public static DependencyProperty AutoStartProperty = DependencyProperty.Register("AutoStart", typeof(bool), typeof(GifImage), new UIPropertyMetadata(false, AutoStartPropertyChanged));

    static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool) e.NewValue)
        {
            var gifImage = (GifImage) sender;
            gifImage.StartAnimation();
        }
    }

    public string GifSource
    {
        get { return (string) GetValue(GifSourceProperty); }
        set
        {
            SetValue(GifSourceProperty, value);
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(value);
                imageSource.EndInit();
                Source = imageSource;
            }
        }
    }

    public static DependencyProperty GifSourceProperty = DependencyProperty.Register("GifSource", typeof(string), typeof(GifImage), new UIPropertyMetadata(string.Empty, GifSourcePropertyChanged));

    static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var gifImage = (GifImage) sender;
        gifImage.Initialize();
    }

    public void StartAnimation()
    {
        if (!isInitialized)
            Initialize();

        BeginAnimation(FrameIndexProperty, animation);
    }

    public void StopAnimation()
    {
        BeginAnimation(FrameIndexProperty, null);
    }
}