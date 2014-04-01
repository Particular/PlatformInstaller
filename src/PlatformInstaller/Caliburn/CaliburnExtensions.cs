using Autofac;
using Caliburn.Micro;
using Parameter = Autofac.Core.Parameter;

public static class CaliburnExtensions
{

    public static T ShowDialog<T>(this IWindowManager windowManager) where T : new()
    {
        var rootModel = new T();
        windowManager.ShowDialog(rootModel);
        return rootModel;
    }

    public static void ActivateModel<T>(this Conductor<object> conductor, params Parameter[] parameters) where T : Screen
    {
        //if (parameters == null)
        //{
        //    parameters = Enumerable.Empty<Parameter>();
        //}
        var model = ContainerFactory.Container.Resolve<T>(parameters);
  
        conductor.ActivateItem(model);
    }
    public static void Publish<T>(this IEventAggregator eventAggregator) where T : new()
    {
        eventAggregator.Publish(new T());
    }
    
}

