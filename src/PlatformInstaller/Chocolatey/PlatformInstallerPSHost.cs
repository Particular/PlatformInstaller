using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Reflection;
using System.Threading;

public class PlatformInstallerPSHost : PSHost
{
    CultureInfo originalCultureInfo = Thread.CurrentThread.CurrentCulture;
    PlatformInstallerPSHostUI hostUI;

    public PlatformInstallerPSHost(PowerShellRunner runner)
    {
        hostUI = new PlatformInstallerPSHostUI(runner);
    }


    public override PSHostUserInterface UI
    {
        get { return hostUI; }
    }

    public override CultureInfo CurrentCulture
    {
        get { return originalCultureInfo; }
    }

    CultureInfo originalUiCultureInfo = Thread.CurrentThread.CurrentUICulture;

    public override CultureInfo CurrentUICulture
    {
        get { return originalUiCultureInfo; }
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