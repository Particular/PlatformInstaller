using System;
using System.Reflection;
using Autofac;
using Caliburn.Micro;

public static class ContainerFactory
{
    public static IContainer Container;

    static ContainerFactory()
    {
        var builder = new ContainerBuilder();
        builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(ThisAssembly())
            .Where(type => (IsInstanceViewModel(type)))
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterType<EventAggregator>()
            .As<IEventAggregator>()
            .SingleInstance();
        builder.RegisterType<WindowManager>()
            .As<IWindowManager>()
            .SingleInstance();
        builder.RegisterType<InstallFeedbackReporter>()
            .SingleInstance();
        builder.RegisterType<ShellViewModel>()
            .SingleInstance();
        builder.RegisterType<PowerShellRunner>()
            .SingleInstance();
        builder.RegisterType<ChocolateyInstaller>()
            .SingleInstance();
        builder.RegisterType<ProcessRunner>()
            .SingleInstance();
        builder.RegisterType<LicenseAgreement>()
            .SingleInstance();
        builder.RegisterType<PackageManager>()
            .SingleInstance();
        builder.RegisterType<PackageDefinitionService>()
            .SingleInstance();
        builder.RegisterType<PendingRestart>()
            .SingleInstance();

        Container = builder.Build();

        Container.Resolve<ChocolateyInstaller>().PatchRunNuget();
        Container.Resolve<PendingRestart>().RemovePendingRestart();

        foreach (var service in Container.GetSingleInstanceRegistrations())
        {
            var instance = Container.Resolve(service);
            SubscribeIfHandler(service, instance);
        }
    }

    static bool IsInstanceViewModel(Type type)
    {
        if (type.Name == typeof(ShellViewModel).Name)
        {
            return false;
        }
        return type.IsView() || type.IsViewModel();
    }

    static void SubscribeIfHandler(Type service, object instance)
    {
        if (service.IsHandler())
        {
            Container.Resolve<IEventAggregator>().Subscribe(instance);
        }
    }


    static Assembly ThisAssembly()
    {
        return typeof(ContainerFactory).Assembly;
    }

    static bool IsViewModel(this Type type)
    {
        return type.Name.EndsWith("ViewModel");
    }

    static bool IsView(this Type type)
    {
        return type.Name.EndsWith("View");
    }
}