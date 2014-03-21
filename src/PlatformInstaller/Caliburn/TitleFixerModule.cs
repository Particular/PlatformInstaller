using Autofac;
using Autofac.Core;
using Caliburn.Micro;

public class TitleFixerModule :Module
{

    protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
    {
        base.AttachToComponentRegistration(componentRegistry, registration);
        registration.Activated += registration_Activated;
    }

    void registration_Activated(object sender, ActivatedEventArgs<object> e)
    {
        var screen = e.Instance as Screen;
        if (screen != null)
        {
            screen.DisplayName = "Particular Platform Installer";
        }
    }
}