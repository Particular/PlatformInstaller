using System;
using System.Threading;
using Caliburn.Micro;

public class NServiceBusPrerequisitesInstallRunner : IInstallRunner
{
    IEventAggregator eventAggregator;

    public NServiceBusPrerequisitesInstallRunner(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
    }

    public int NestedActionCount
    {
        get { return 3; }
    }

    public string Status()
    {
        return string.Empty;
    }

    public void GetReleaseInfo()
    {
        throw new NotSupportedException("This should not be called for Prerequisites");
    }

    public bool HasReleaseInfo()
    {
        throw new NotSupportedException("This should not be called for Prerequisites");
    }

    public int InstallationResult { get; private set; }

    public Version CurrentVersion()
    {
        throw new NotSupportedException();
    }

    public Version LatestAvailableVersion()
    {
        throw new NotSupportedException();
    }

    public void Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
                                          {
                                              Name = "NServiceBus Prerequisites - Microsoft Message Queue (MSMQ)"
                                          });
        if (!MsmqSetupStep(logOutput, logError))
        {
            return;
        }
        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());

        Thread.Sleep(1000);

        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
                                          {
                                              Name = "NServiceBus Prerequisites - Distributed Transaction Co-ordinator"
                                          });
        if (!DtcSetupStep(logOutput, logError))
            return;
        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());

        Thread.Sleep(1000);

        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
                                          {
                                              Name = "NServiceBus Prerequisites - Performance Counters"
                                          });
        if (!PerfCounterSetupStep(logOutput, logError))
            return;
        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());

        Thread.Sleep(1000);
        InstallationResult = 0;

    }

    public bool Installed()
    {
        return true;
    }

    bool PerfCounterSetupStep(Action<string> logOutput, Action<string> logError)
    {
        try
        {
            logOutput("Checking NServiceBus Performance Counters");
            var setup = new PerfCountersInstaller();

            var allCountersExist = setup.CheckCounters();
            if (allCountersExist)
            {
                logOutput("Performance Counters OK");
                return true;
            }
            logOutput("Adding NServiceBus Performance Counters");
            if (setup.DoesCategoryExist())
            {
                setup.DeleteCategory();
            }
            setup.SetupCounters();
        }
        catch (Exception ex)
        {
            logError("NServiceBus Performance Counters install failed:");
            logError(string.Format("{0}", ex));
            return false;
        }
        return true;
    }

    bool DtcSetupStep(Action<string> logOutput, Action<string> logError)
    {
        try
        {
            var dtc = new DtcInstaller(logOutput);
            if (!dtc.IsDtcWorking())
            {
                dtc.ReconfigureAndRestartDtcIfNecessary();
            }
        }
        catch (Exception ex)
        {
            logError("DTC install has failed:");
            logError(string.Format("{0}", ex));
            return false;
        }
        return true;
    }

    bool MsmqSetupStep(Action<string> logOutput, Action<string> logError)
    {
        try
        {
            var msmq = new MsmqInstaller(logOutput);

            if (msmq.IsInstalled())
            {
                if (msmq.IsInstallationGood())
                {
                    logOutput("MSMQ configuration OK");
                }
                else
                {
                    logError("MSMQ is already installed but has unsupported options enabled.");
                    logError(string.Format("To use NServiceBus please disable any of the following MSMQ components:\r\n {0} \r\n", string.Join(",\r\n", msmq.UnsupportedComponents())));
                    return false;
                }
            }
            else
            {
                msmq.InstallMsmq();
            }

            if (!msmq.StartMsmqIfNecessary())
            {
                logError("MSMQ Service did not start");
                eventAggregator.Publish<RebootRequiredEvent>();
                return false;
            }
        }
        catch (Exception ex)
        {
            logError("MSMQ install has failed:");
            logError(string.Format("{0}", ex));
            return false;
        }
        return true;
    }

}