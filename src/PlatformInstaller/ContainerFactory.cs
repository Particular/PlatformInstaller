using System.Linq;
using Autofac;
using Autofac.Core;
using Caliburn.Micro;

public static class ContainerFactory
{
    public static IContainer Container;

    static ContainerFactory()
    {
        var builder = new ContainerBuilder();
        builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
            .Where(type => type.Name.EndsWith("ViewModel"))
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
            .Where(type => type.Name.EndsWith("View"))
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterType<EventAggregator>()
            .As<IEventAggregator>()
            .SingleInstance();
        builder.RegisterType<ProgressService>()
            .SingleInstance();
        builder.RegisterType<Logging>()
            .SingleInstance();
        builder.RegisterType<WindowManager>()
            .As<IWindowManager>()
            .SingleInstance();
        builder.RegisterType<NewUserDetecter>()
            .SingleInstance();
        builder.RegisterType<PowerShellRunner>()
            .SingleInstance();
        builder.RegisterType<ChocolateyInstaller>()
            .SingleInstance();
        builder.RegisterType<ProcessRunner>()
            .SingleInstance();
        builder.RegisterType<PlatformInstallerPSHost>()
            .SingleInstance();
        builder.RegisterType<PlatformInstallerPSHostUI>()
            .SingleInstance();
        builder.RegisterType<PackageManager>()
            .SingleInstance();
        builder.RegisterType<PackageDefinitionService>()
            .SingleInstance();

        Container = builder.Build();
        Container.Resolve<Logging>();
    }
}

public class Foo :Module
{
    protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
    {
        base.AttachToComponentRegistration(componentRegistry, registration);
        registration.Activated += registration_Activated;
    }

    void registration_Activated(object sender, ActivatedEventArgs<object> e)
    {
        if (e.Instance.GetType().Name.EndsWith("ViewModel"))
        {
            ContainerFactory.Container.Resolve<IEventAggregator>().Subscribe(e.Instance);
        }
    }
}