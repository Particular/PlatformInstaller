using System;
using System.Linq;
using Caliburn.Micro;

public static class CaliburnExtensions
{
    public static void Publish<T>(this IEventAggregator eventAggregator) where T : new()
    {
        eventAggregator.PublishOnUIThread(new T());
    }
    
    public static bool IsHandler(this Type type)
    {
        return type.GetInterfaces().Any(x => x.Name.Contains("IHandle"));
    }
 
    public static bool IsHandler(this object screen)
    {
        return screen.GetType().GetInterfaces().Any(x => x.Name.Contains("IHandle"));
    }
}

