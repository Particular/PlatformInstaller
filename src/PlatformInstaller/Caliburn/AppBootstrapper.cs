﻿using System;
using Autofac;
using Caliburn.Micro;
using PlatformInstaller;

public class AppBootstrapper : Bootstrapper<ShellViewModel>
{
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