using System;
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

    public void Init()
    {
         // No init necessary
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

    public async Task Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
        {
            Name = "NServiceBus Prerequisites - Microsoft Message Queue (MSMQ)"
        });

        await MsmqSetupStep(logOutput, logError).
            ConfigureAwait(false);

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());

        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
        {
            Name = "NServiceBus Prerequisites - Distributed Transaction Coordinator"
        });

        await DtcSetupStep(logOutput, logError)
            .ConfigureAwait(false);

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());

        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent{Name = "NServiceBus Prerequisites - Performance Counters"});

        await Task.Run(() => { PerfCounterSetupStep(logOutput, logError); }).ConfigureAwait(false);

        eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());

        //Give the user a chance to see the final text
        await Task.Delay(2000)
            .ConfigureAwait(false);

    }

    public bool Installed()
    {
        return false;
    }

    void PerfCounterSetupStep(Action<string> logOutput, Action<string> logError)
    {
        try
        {
            logOutput("Checking NServiceBus Performance Counters");
            var setup = new PerfCountersInstaller();

            var allCountersExist = setup.CheckCounters();
            if (allCountersExist)
            {
                logOutput("Performance Counters OK");
                return;
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
        }

    }

    async Task DtcSetupStep(Action<string> logOutput, Action<string> logError)
    {

        try
        {
            var dtc = new DtcInstaller(logOutput);
            if (!dtc.IsDtcWorking())
            {
                await dtc.ReconfigureAndRestartDtcIfNecessary()
                    .ConfigureAwait(false);
            }

        }
        catch (Exception ex)
        {
            logError("DTC install has failed:");
            logError($"{ex}");
        }
        await Task.Delay(2000)
            .ConfigureAwait(false);
    }

    async Task MsmqSetupStep(Action<string> logOutput, Action<string> logError)
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
                    return;
                }
            }
            else
            {
                msmq.InstallMsmq();
            }

            if (!await msmq.StartMsmqIfNecessary().ConfigureAwait(false))
            {
                logError("MSMQ Service did not start");
                eventAggregator.Publish<RebootRequiredEvent>();
            }
        }
        catch (Exception ex)
        {
            logError("MSMQ install has failed:");
            logError($"{ex}");
        }
    }
}