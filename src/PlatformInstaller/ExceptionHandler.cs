using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Anotar.Serilog;

static class ExceptionHandler
{
    static Dispatcher dispatcher;
    static bool errorDialogShown;
    public static void Attach()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        dispatcher = Dispatcher.CurrentDispatcher;
        dispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
        Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    public static void HandleException(Exception exception, string message)
    {
        if (exception == null)
        {
            LogTo.Error(message);
        }
        else
        {
            LogTo.Error(exception, message);
        }
        try
        {
            if (errorDialogShown)
            {
                return;
            }
            errorDialogShown = true;
            dispatcher.Invoke(() =>
            {
                var exceptionView = new ExceptionView(exception)
                {
                    Owner = ShellView.CurrentInstance
                };
                exceptionView.ShowDialog();
            });
        }
        catch (Exception showDialogException)
        {
            LogTo.Error(showDialogException, "Could not show error dialog. Shutting down.");
            Environment.Exit(1);
        }
        finally
        {
            errorDialogShown = false;
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
        if (e.IsTerminating)
        {
            LogTo.Error(e.ExceptionObject as Exception, "Could not show error dialog. Shutting down.");
            return;
        }
        HandleException(e.ExceptionObject as Exception, "CurrentDomain_UnhandledException");
    }

}