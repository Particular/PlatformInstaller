using System;
using Autofac;
using Caliburn.Micro;

public class AppBootstrapper : BootstrapperBase
{

    ILifetimeScope lifetimeScope;

    public AppBootstrapper(ILifetimeScope lifetimeScope):base(useApplication: false)
    {
        this.lifetimeScope = lifetimeScope;
        ViewLocator.LocateTypeForModelType = (modelType, dependencyObject, arg3) => GetViewForModel(modelType);
    }

    public static Type GetViewForModel(Type type)
    {
        var viewName = type.Name.Replace("Model", "");
        return Type.GetType(viewName, true);
    }

    protected override object GetInstance(Type service, string key)
    {
        object instance;
        if (string.IsNullOrWhiteSpace(key))
        {
            if (lifetimeScope.TryResolve(service, out instance))
            {
                return instance;
            }
        }
        else
        {
            if (lifetimeScope.TryResolveNamed(key, service, out instance))
            {
                return instance;
            }
        }
        throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
    }
}