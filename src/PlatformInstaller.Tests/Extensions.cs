using System;
using System.Threading;

public static class Extensions
{
    public static void ThrowIfHandleTimesOut(this WaitHandle handle, TimeSpan maxWaitTime)
    {
        var signalReceived = handle.WaitOne(maxWaitTime);
        if (!signalReceived)
        {
            throw new Exception("Timeout while waiting on handle");
        }
    }
}