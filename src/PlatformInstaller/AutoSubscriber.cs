using System;
using Autofac;
using Caliburn.Micro;

public class AutoSubscriber
{
    ILifetimeScope lifetimeScope;
    IEventAggregator eventAggregator;

    public AutoSubscriber(ILifetimeScope lifetimeScope, IEventAggregator eventAggregator)
    {
        this.lifetimeScope = lifetimeScope;
        this.eventAggregator = eventAggregator;
    }

    public void SubScribe()
    {
        foreach (var service in lifetimeScope.GetSingleInstanceRegistrations())
        {
            var instance = lifetimeScope.Resolve(service);
            SubscribeIfHandler(service, instance);
        }
    }

    void SubscribeIfHandler(Type service, object instance)
    {
        if (service.IsHandler())
        {
            eventAggregator.Subscribe(instance);
        }
    }

}