using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Mindscape.Raygun4Net;

public class AppBootstrapper : BootstrapperBase
{
    ILifetimeScope lifetimeScope;

    public AppBootstrapper()
    {
        Initialize();
    }

    protected override void Configure()
    {
        lifetimeScope = BuildContainer();
    }

    static Assembly ThisAssembly()
    {
        return typeof(AppBootstrapper).Assembly;
    }

    public static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();
        builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();
        builder.RegisterAssemblyTypes(ThisAssembly())
            .Where(ViewModelConventions.IsInstanceViewOrModel)
            .AsSelf()
            .InstancePerDependency();
        builder.RegisterType<EventAggregator>()
            .As<IEventAggregator>()
            .SingleInstance();
        builder.RegisterType<WindowManager>()
            .As<IWindowManager>()
            .SingleInstance();
        builder.RegisterType<AbortInstallationHandler>()
            .SingleInstance();
        builder.RegisterType<InstallFeedbackReporter>()
            .SingleInstance();
        builder.RegisterType<ShellViewModel>()
            .SingleInstance();
        builder.RegisterType<ProcessRunner>()
            .SingleInstance();
        builder.RegisterType<CredentialStore>()
            .SingleInstance();
        builder.RegisterType<ProxyTester>()
            .SingleInstance();
        builder.RegisterType<ReleaseManager>()
            .SingleInstance();
        builder.RegisterType<Installer>()
            .SingleInstance();
        builder.RegisterType<LicenseAgreement>()
            .SingleInstance();
        builder.RegisterType<AutoSubscriber>();
        builder.RegisterAssemblyTypes(ThisAssembly())
            .Where(t => t.GetInterfaces()
            .Any(i => i.IsAssignableFrom(typeof(IInstaller))))
            .SingleInstance().AsImplementedInterfaces();
        builder.RegisterInstance(new RaygunClient(App.RayGunApiKey))
            .SingleInstance();
        builder.RegisterType<RuntimeUpgradeManager>()
            .SingleInstance();
        return builder.Build();
    }


    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        ViewLocator.LocateTypeForModelType = (modelType, dependencyObject, arg3) => ViewModelConventions.GetViewForModel(modelType);
        lifetimeScope.Resolve<AutoSubscriber>().Subscribe();
        lifetimeScope.Resolve<IWindowManager>().ShowDialog(lifetimeScope.Resolve<ShellViewModel>());
        base.OnStartup(sender, e);
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