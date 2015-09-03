﻿using System;
using System.Threading.Tasks;
using Caliburn.Micro;

public class NServiceBusPrerequisitesInstaller : IInstaller
{
    IEventAggregator eventAggregator;

    public NServiceBusPrerequisitesInstaller(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
    }

    public int NestedActionCount
    {
        get { return 3; }
    }

    public string Name { get { return "NServiceBus Pre-requisites"; }}

    public string Status
    {
        get { return string.Empty; }
    }


    public string ToolTip
    {
        get
        {
            return "Configure MSMQ, DTC and install NServiceBus Performance Counters";
        }
    }

    public bool SelectedByDefault { get { return true; } }
    public bool Enabled { get { return true; } }

    public Version CurrentVersion()
    {
        throw new NotSupportedException();
    }

    public Version LatestAvailableVersion()
    {
        throw new NotSupportedException();
    }

    public Task Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
                                          {
                                              Name = "NServiceBus Prerequisites - Microsoft Message Queue (MSMQ)"
                                          });
        if (!MsmqSetupStep(logOutput, logError))
        {
            return Task.FromResult(0);
        }
        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());

        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
                                          {
                                              Name = "NServiceBus Prerequisites - Distributed Transaction Coordinator"
                                          });
        if (!DtcSetupStep(logOutput, logError))
        {
            return Task.FromResult(0);
        }
        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());

        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
                                          {
                                              Name = "NServiceBus Prerequisites - Performance Counters"
                                          });
        if (!PerfCounterSetupStep(logOutput, logError))
        {
            return Task.FromResult(0);
        }
        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
        
        return Task.FromResult(0);
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