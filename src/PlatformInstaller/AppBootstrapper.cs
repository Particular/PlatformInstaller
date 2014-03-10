using System;
using System.Linq;
using Autofac;
using Caliburn.Micro;
using System.Linq;

namespace PlatformInstaller
{
    public class AppBootstrapper : Bootstrapper<MainViewModel>
    {
        static IContainer Container;
        protected override void Configure()
        {
            var builder = new ContainerBuilder();
            builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();

            builder.RegisterType<HardcodedPackageService>().AsImplementedInterfaces().SingleInstance();
            
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
              .Where(type => type.Name.EndsWith("ViewModel"))
              .AsSelf()
              .InstancePerDependency();

            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
              .Where(type => type.Name.EndsWith("View"))
              .AsSelf()
              .InstancePerDependency();

            Container = builder.Build();
        }

        protected override object GetInstance(Type service, string key)
        {
            object instance;
            if (string.IsNullOrWhiteSpace(key))
            {
                if (ContainerFactory.Container.TryResolve(service, out instance))
                    return instance;
            }
            else
            {
                if (ContainerFactory.Container.TryResolveNamed(key, service, out instance))
                    return instance;
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }
    }
}