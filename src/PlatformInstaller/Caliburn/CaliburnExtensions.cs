using System;
using Autofac;
using Caliburn.Micro;

public static class CaliburnExtensions
{

    public static T ShowDialog<T>(this IWindowManager windowManager) where T : new()
    {
        var rootModel = new T();
        windowManager.ShowDialog(rootModel);
        return rootModel;
    }

    public static void ActivateModel<T>(this Conductor<object> conductor, Action<T> initialise = null) where T: Screen
    {
        var model = ContainerFactory.Container.Resolve<T>();
        if (initialise != null)
        {
            initialise(model);
        }
        conductor.ActivateItem(model);
    }
    public static void Publish<T>(this IEventAggregator eventAggregator) where T : new()
    {
        eventAggregator.Publish(new T());
    }
    
}

