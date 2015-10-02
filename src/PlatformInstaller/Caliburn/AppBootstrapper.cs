using System;
using Autofac;
using Caliburn.Micro;

public class AppBootstrapper : BootstrapperBase
{

    ILifetimeScope lifetimeScope;

    public AppBootstrapper(ILifetimeScope lifetimeScope):base(useApplication: false)
    {
        this.lifetimeScope = lifetimeScope;
        ViewLocator.LocateTypeForModelType = (modelType, dependencyObject, arg3) =>ViewModelConventions.GetViewForModel(modelType);
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
        throw new Exception($"Could not locate any instances of contract {key ?? service.Name}.");
    }
}