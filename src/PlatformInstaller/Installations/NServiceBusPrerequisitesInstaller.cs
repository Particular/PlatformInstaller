﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;

public class NServiceBusPrerequisitesInstaller : IInstaller
{
    IEventAggregator eventAggregator;

    public NServiceBusPrerequisitesInstaller(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
    }


    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield break;
    }

    public IEnumerable<DocumentationLink> GetDocumentationLinks()
    {
        yield return new DocumentationLink
        {
            Text = "NServiceBus documentation",
            Url = "http://docs.particular.net/nservicebus/"
        };
        yield return new DocumentationLink
        {
            Text = "NServiceBus Samples",
            Url = "http://docs.particular.net/samples/"
        };
    }

    public int NestedActionCount => 3;

    public string Name => "NServiceBus Pre-requisites";

    public string Status => string.Empty;

    public string ToolTip => "Configure MSMQ, DTC and install NServiceBus Performance Counters";

    public bool SelectedByDefault => true;
    public bool Enabled => true;

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
            logError($"{ex}");
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
            logError($"{ex}");
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
                    logError($"To use NServiceBus please disable any of the following MSMQ components:\r\n {string.Join(",\r\n", msmq.UnsupportedComponents())} \r\n");
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
            logError($"{ex}");
            return false;
        }
        return true;
    }

}