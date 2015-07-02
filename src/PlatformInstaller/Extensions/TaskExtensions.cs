using System;
using System.Net;
using System.Threading.Tasks;

static class TaskExtensions
{
    public static Task<T> Catch<T, TError>(this Task<T> task, Func<TError, T> onError) where TError : Exception
    {
        var tcs = new TaskCompletionSource<T>();

        task.ContinueWith(ant =>
        {
            if (task.IsFaulted && task.Exception.InnerException is TError)
                tcs.SetResult(onError((TError)task.Exception.InnerException));
            else if (ant.IsCanceled)
                tcs.SetCanceled();
            else if (task.IsFaulted)
                tcs.SetException(ant.Exception.InnerException);
            else
                tcs.SetResult(ant.Result);
        });
        return tcs.Task;
    }

    public static Task<WebResponse> CatchWebException(this Task<WebResponse> task)
    {
        return task.Catch<WebResponse, WebException>(e => e.Response);
    }
}