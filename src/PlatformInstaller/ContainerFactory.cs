using System.Linq;
using Autofac;
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

        builder.RegisterType<ProgressService>()
            .SingleInstance();
        builder.RegisterType<WindowManager>()
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
    }
}