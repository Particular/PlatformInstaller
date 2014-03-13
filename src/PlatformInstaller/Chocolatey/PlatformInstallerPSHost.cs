using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Reflection;
using System.Threading;

public class PlatformInstallerPSHost : PSHost
{
    PlatformInstallerPSHostUI hostUI;

    public PlatformInstallerPSHost(PlatformInstallerPSHostUI hostUI)
    {
        this.hostUI = hostUI;
    }

    public override PSHostUserInterface UI
    {
        get { return hostUI; }
    }

    public override CultureInfo CurrentCulture
    {
        get { return Thread.CurrentThread.CurrentCulture; }
    }

    public override CultureInfo CurrentUICulture
    {
        get { return Thread.CurrentThread.CurrentUICulture; }
    }

    Guid hostInstanceId = Guid.NewGuid();

    public override Guid InstanceId
    {
        get { return hostInstanceId; }
    }

    public override string Name
    {
        get { return @"Platform Installer Host"; }
    }

    public override void SetShouldExit(int exitCode)
    {
        throw new NotImplementedException();
    }
    public override Version Version
    {
        get { return Assembly.GetExecutingAssembly().GetName().Version; }
    }

    public override void NotifyBeginApplication()
    {
    }

    public override void NotifyEndApplication()
    {
    }

    public override void EnterNestedPrompt()
    {
        throw new NotImplementedException();
    }

    public override void ExitNestedPrompt()
    {
        throw new NotImplementedException();
    }

    
}