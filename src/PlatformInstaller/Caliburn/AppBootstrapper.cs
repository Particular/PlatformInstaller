using System;
using Autofac;
using Caliburn.Micro;

public class AppBootstrapper : Bootstrapper<ShellViewModel>
{
    public AppBootstrapper()
    {
        ViewLocator.LocateTypeForModelType = (o, dependencyObject, arg3) =>
        {
            var viewName = o.Name.Replace("Model","");
            return Type.GetType(viewName,true);
        };
    }
    protected override object GetInstance(Type service, string key)
    {
        object instance;
        if (string.IsNullOrWhiteSpace(key))
        {
            if (ContainerFactory.Container.TryResolve(service, out instance))
            {
                return instance;
            }
        }
        else
        {
            if (ContainerFactory.Container.TryResolveNamed(key, service, out instance))
            {
                return instance;
            }
        }
        throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
    }
}