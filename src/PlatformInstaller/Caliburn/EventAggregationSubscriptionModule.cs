using System.Linq;
using Autofac;
using Autofac.Core;
using Caliburn.Micro;

public class EventAggregationSubscriptionModule :Module
{

    protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
    {
        base.AttachToComponentRegistration(componentRegistry, registration);
        registration.Activated += registration_Activated;
    }

    void registration_Activated(object sender, ActivatedEventArgs<object> e)
    {
        if (e.Instance.GetType().GetInterfaces().Any(x=>x.Name.Contains("IHandle")))
        {
            ContainerFactory.Container.Resolve<IEventAggregator>().Subscribe(e.Instance);
        }
    }
}