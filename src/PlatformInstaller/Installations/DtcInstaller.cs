using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Win32;

public class DtcInstaller : IInstaller
{
    IEventAggregator eventAggregator;
    const string RegPathDTCSecurity = @"SOFTWARE\Microsoft\MSDTC\Security";
    RegistryView regview;

    public DtcInstaller(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        regview = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Default;
    }

    public Version CurrentVersion()
    {
        throw new NotSupportedException();
    }

    public Version LatestAvailableVersion()
    {
        throw new NotSupportedException();
    }

    public void Init()
    {
        using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regview))
        using (var dtcKey = localMachine.OpenSubKey(RegPathDTCSecurity, true))
        {
            if (dtcKey == null)
            {
                throw new Exception($@"Registry key not found: HKEY_LOCAL_MACHINE\{RegPathDTCSecurity}");
            }
            InstallState = RegValues.All(p => (int)dtcKey.GetValue(p, 0) == 1) ? InstallState.Installed : InstallState.NotInstalled;
        }
    }

    public async Task Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = Name});
        try
        {
            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regview))
            using (var dtcKey = localMachine.OpenSubKey(RegPathDTCSecurity, true))
            {
                if (dtcKey == null)
                {
                    throw new Exception($@"Registry key not found: HKEY_LOCAL_MACHINE\{RegPathDTCSecurity}");
                }

                foreach (var regValue in RegValues.Where(p => (int) dtcKey.GetValue(p, 0) != 1).ToList())
                {
                    dtcKey.SetValue(regValue, 1, RegistryValueKind.DWord);
                    logOutput($@"HKEY_LOCAL_MACHINE\{RegPathDTCSecurity}\{regValue} set to 1");
                }
            }
            logOutput($"Attempting restart of {Controller.DisplayName}");
            await Controller.ChangeServiceStatus(ServiceControllerStatus.Stopped, Controller.Stop)
                .ConfigureAwait(false);
            await Controller.ChangeServiceStatus(ServiceControllerStatus.Running, Controller.Start)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logError("DTC install has failed:");
            logError($"{ex}");
        }
        finally
        {
            eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
        }
    }

    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield break;
    }

    public IEnumerable<DocumentationLink> GetDocumentationLinks()
    {
        yield break;
    }

    public string  Name => "Configure MSDTC for NServiceBus";
    public string ImageName => "NServiceBus";
    public string Description => "Optional Install";
    public int NestedActionCount => 1;
    public bool RebootRequired => false;
    public string Status => InstallState == InstallState.Installed ? "Installed": "Configure MSDTC";
    public bool SelectedByDefault => false;
    public InstallState InstallState { get; private set; }

    public bool Working => Controller.Status == ServiceControllerStatus.Running;

    static ServiceController Controller = new ServiceController { ServiceName = "MSDTC" };

    static List<string> RegValues = new List<string>
    {
        "NetworkDtcAccess",
        "NetworkDtcAccessOutbound",
        "NetworkDtcAccessTransactions",
        "XaTransactions"
    };
}
