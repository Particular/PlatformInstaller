using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;

public static class AutofacExtensions
{
    public static IEnumerable<Type> GetSingleInstanceRegistrations(this ILifetimeScope container)
    {
        return container.ComponentRegistry.Registrations
            .Where(x => x.Lifetime is RootScopeLifetime)
            .SelectMany(x => x.Services)
            .OfType<IServiceWithType>()
            .Select(x => x.ServiceType);
    }
}