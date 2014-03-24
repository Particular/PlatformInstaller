using System;

public static class AsyncErrorHandler
{
    public static void HandleException(Exception exception)
    {
        ExceptionHandler.HandleException(exception, "AsyncErrorHandler");
    }
}