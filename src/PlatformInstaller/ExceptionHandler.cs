using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Anotar.Serilog;

static class ExceptionHandler
{
    public static void Attach()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
        Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    static void HandleException(Exception exception, string message)
    {
        if (exception == null)
        {
            LogTo.Error(message);
        }
        else
        {
            LogTo.Error(exception, message);
        }
    }

    static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        HandleException(e.Exception, "TaskScheduler.UnobservedTaskException");
    }


    static void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        HandleException(e.Exception, "Application.Current.DispatcherUnhandledException");
    }

    static void CurrentDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        HandleException(e.Exception, "Dispatcher.CurrentDispatcher.UnhandledException");
    }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        HandleException(e.ExceptionObject as Exception, "CurrentDomain_UnhandledException");
    }

}