using Autofac;
using Caliburn.Micro;

public class AbortInstallationHandler : IHandle<RequestExitApplicationEvent>,
    IHandle<InstallFailedEvent>,
    IHandle<InstallSucceededEvent>,
    IHandle<InstallStartedEvent>
{
    IEventAggregator eventAggregator;
    IWindowManager windowManager;
    ILifetimeScope lifetimeScope;
    bool isInstalling;

    public AbortInstallationHandler(IEventAggregator eventAggregator, IWindowManager windowManager, ILifetimeScope lifetimeScope)
    {
        this.eventAggregator = eventAggregator;
        this.windowManager = windowManager;
        this.lifetimeScope = lifetimeScope;
    }

    public void Handle(RequestExitApplicationEvent message)
    {
        if (isInstalling)
        {
            using (var beginLifetimeScope = lifetimeScope.BeginLifetimeScope())
            {
                var confirmModel = beginLifetimeScope.Resolve<ConfirmAbortInstallViewModel>();
                windowManager.ShowDialog(confirmModel);
                if (confirmModel.AbortInstallation)
                {
                    eventAggregator.Publish<CancelInstallCommand>();
                    eventAggregator.Publish<ExitApplicationCommand>();
                }
                return;
            }
        }
        eventAggregator.Publish<ExitApplicationCommand>();
    }


    public void Handle(InstallStartedEvent message)
    {
        isInstalling = true;
    }

    public void Handle(InstallFailedEvent message)
    {
        isInstalling = false;
    }

    public void Handle(InstallSucceededEvent message)
    {
        isInstalling = false;
    }
}