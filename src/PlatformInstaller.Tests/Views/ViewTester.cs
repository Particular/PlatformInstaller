using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ApprovalTests;
using ApprovalTests.Wpf;
using ApprovalUtilities.Wpf;
using Caliburn.Micro;

public static class ViewTester
{
    public static void ShowView<T>(T model) where T : INotifyPropertyChanged
    {
        var window = GetWindow(model);
        window.ShowDialog();
    }

    public static void VerifyView<T>(T model) where T : INotifyPropertyChanged
    {
        var window = GetWindow(model);
        Approvals.Verify(new ImageWriter(f => WpfUtils.ScreenCapture(window, f)));
    }

    public static void ScreenCapture<T>(T model) where T : INotifyPropertyChanged
    {
        var window = GetWindow(model);
        var filename = $"{typeof(T).Name.Replace("ViewModel", "")}.png";
        WpfUtils.ScreenCapture(window, filename);
    }

    static Window GetWindow<T>(T model) where T : INotifyPropertyChanged
    {
        var viewType = ViewModelConventions.GetViewForModel(typeof(T));
        var view = (ContentControl) Activator.CreateInstance(viewType);
        var window = new Window
            {
                Content = view,
                Height = 400,
                Width = 400,
            };
        ViewModelBinder.Bind(model, view, null);
        return window;
    }
}