using System.Reflection;
using Autofac;
using Caliburn.Micro;
using Mindscape.Raygun4Net;

public static class ContainerFactory
{
    public static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();
        builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(ThisAssembly())
            .Where(type => (ViewModelConventions.IsInstanceViewOrModel(type)))
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterType<EventAggregator>()
            .As<IEventAggregator>()
            .SingleInstance();
        builder.RegisterType<WindowManager>()
            .As<IWindowManager>()
            .SingleInstance();
        builder.RegisterType<RebootMachine>()
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
        builder.RegisterType<PendingRestartAndResume>()
            .SingleInstance();
        builder.RegisterInstance(new RaygunClient(Program.RaygunApiKey))
            .SingleInstance();
        
        return builder.Build();
    }

    static Assembly ThisAssembly()
    {
        return typeof(ContainerFactory).Assembly;
    }
}