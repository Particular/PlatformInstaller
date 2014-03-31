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
        builder.RegisterModule<EventAggregationSubscriptionModule>();
        builder.RegisterModule<TitleFixerModule>();
        builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(ThisAssembly())
            .Where(type => type.IsView() || type.IsViewModel())
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

        Container = builder.Build();

        //hack: since nothing uses InstallFeedbackReporter force a resolve
        Container.Resolve<InstallFeedbackReporter>();
    }

    static Assembly[] ThisAssembly()
    {
        return new[]{typeof(ContainerFactory).Assembly};
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