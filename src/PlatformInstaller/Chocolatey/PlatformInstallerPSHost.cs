using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Reflection;

public class PlatformInstallerPSHost : PSHost
{
    private readonly CultureInfo _originalCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
    private readonly PlatformInstallerPSHostUI _hostUI;

    public PlatformInstallerPSHost(PowerShellRunner runner)
    {
        _hostUI = new PlatformInstallerPSHostUI(runner);
    }


    public override PSHostUserInterface UI
    {
        get { return _hostUI; }
    }

    public override CultureInfo CurrentCulture
    {
        get { return _originalCultureInfo; }
    }

    private readonly CultureInfo _originalUiCultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture;

    public override CultureInfo CurrentUICulture
    {
        get { return _originalUiCultureInfo; }
    }

    private readonly Guid _hostInstanceId = Guid.NewGuid();

    public override Guid InstanceId
    {
        get { return _hostInstanceId; }
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