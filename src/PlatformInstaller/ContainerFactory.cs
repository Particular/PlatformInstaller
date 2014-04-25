using System;
using System.Reflection;
using Autofac;
using Caliburn.Micro;

public static class ContainerFactory
{
    public static IContainer BuildContainer()
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
        builder.RegisterType<AppBootstrapper>()
            .SingleInstance();
        builder.RegisterType<AutoSubscriber>();
        builder.RegisterType<PackageDefinitionService>()
            .SingleInstance();
        builder.RegisterType<PendingRestart>()
            .SingleInstance();

        var container = builder.Build();


        return container;
    }

    static bool IsInstanceViewModel(Type type)
    {
        if (type.Name == typeof(ShellViewModel).Name)
        {
            return false;
        }
        return type.IsView() || type.IsViewModel();
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