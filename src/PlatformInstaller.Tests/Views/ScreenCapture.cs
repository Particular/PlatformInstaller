using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using ApprovalUtilities.Wpf;
using Caliburn.Micro;
using Size = System.Drawing.Size;

public static class ScreenCapture
{
    public static void TakeScreenShot(this Screen model)
    {
        ShellViewModel.StartModel = model;
        var app = new App();
        app.Activated += (o, args) =>
        {

            WpfUtils.ScreenCapture(app.MainWindow, model.GetType().Name + ".png");

            //Window mainWindow = app.MainWindow;
            //mainWindow.BringIntoView();
            //var bmp = new Bitmap((int) mainWindow.Width, (int)mainWindow.Height, PixelFormat.Format32bppArgb);
            //Graphics graphics = Graphics.FromImage(bmp);
            //graphics.CopyFromScreen((int)mainWindow.Left, (int)mainWindow.Top, 0, 0, new Size((int)mainWindow.Width, (int)mainWindow.Height), CopyPixelOperation.SourceCopy);

            //bmp.Save("test.png", ImageFormat.Png);
            //app.Shutdown();
        };
        app.InitializeComponent();
        app.Run();
    }

    public static void CaptureApplication(string procName)
    {
        var proc = Process.GetProcessesByName(procName)[0];
        var rect = new Rect();
        GetWindowRect(proc.MainWindowHandle, ref rect);

        int width = rect.right - rect.left;
        int height = rect.bottom - rect.top;

        var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        Graphics graphics = Graphics.FromImage(bmp);
        graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

        bmp.Save("c:\\tmp\\test.png", ImageFormat.Png);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

}