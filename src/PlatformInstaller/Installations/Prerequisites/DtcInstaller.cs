using System;
using System.Collections.Generic;
using System.ServiceProcess;
using Microsoft.Win32;

public class DtcInstaller
{
    Action<string> output;

    public DtcInstaller(Action<string> output)
    {
        this.output = output;
        output("Checking Distributed Transaction Co-Ordinater Configuration");
    }

    public void ReconfigureAndRestartDtcIfNecessary()
    {
        var processUtil = new ProcessUtil();

        if (DoesSecurityConfigurationRequireRestart(true))
        {
            output("Stopping DTC service");
            processUtil.ChangeServiceStatus(Controller, ServiceControllerStatus.Stopped, Controller.Stop);
        }
        output("Starting DTC service");
        processUtil.ChangeServiceStatus(Controller, ServiceControllerStatus.Running, Controller.Start);
    }

    public bool IsDtcWorking()
    {
        if (DoesSecurityConfigurationRequireRestart(false))
        {
            output("DTC requires a restart");
            return false;
        }

        if (Controller.Status != ServiceControllerStatus.Running)
        {
            output("DTC service is not running");
            return false;
        }
        output("DTC configuration OK");
        return true;
    }

    bool DoesSecurityConfigurationRequireRestart(bool doChanges)
    {
        var regview = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Default;
        using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regview))
        {
            const string keyName = @"SOFTWARE\Microsoft\MSDTC\Security";
            var requireRestart = false;
            using (var dtcKey = localMachine.OpenSubKey(keyName, true))
            {
                if (dtcKey == null)
                {
                    throw new Exception(string.Format(@"Registry key noy found: HKEY_LOCAL_MACHINE\{0}", keyName));
                }
                foreach (var val in RegValues)
                {
                    if ((int) dtcKey.GetValue(val, 0) != 0)
                    {
                        continue;
                    }

                    if (doChanges)
                    {
                        if (!dtcKey.TrySetValue(val, 1, RegistryValueKind.DWord))
                        {
                            throw new Exception(string.Format("Failed to set value '{0}' to '{1}' in '{2}'", val, 1, keyName));
                        }
                        output(string.Format("Set value '{0}' to '{1}' in '{2}'", val, 1, keyName));
                    }
                    requireRestart = true;
                }
            }
            return requireRestart;
        }
    }

    static readonly ServiceController Controller = new ServiceController { ServiceName = "MSDTC", MachineName = "." };
    static readonly List<string> RegValues = new List<string>(new[] { "NetworkDtcAccess", "NetworkDtcAccessOutbound", "NetworkDtcAccessTransactions", "XaTransactions" });
}
