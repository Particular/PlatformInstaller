using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

public class PerfCountersInstaller : IInstaller
{
    IEventAggregator eventAggregator;
    public PerfCountersInstaller(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
    }

    public string ImageName => "NServiceBus";

    public Version CurrentVersion()
    {
        throw new NotSupportedException();
    }

    public Version LatestAvailableVersion()
    {
        throw new NotSupportedException();
    }

    public InstallState InstallState { get; private set; }

    public void Init()
    {
        var installed = PerformanceCounterCategory.Exists(categoryName) && Counters.All(counter => PerformanceCounterCategory.CounterExists(counter.CounterName, categoryName));
        InstallState = installed ? InstallState.Installed : InstallState.NotInstalled;
    }

    public async Task Execute(Action<string> logOutput, Action<string> logError)
    {
        eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent { Name = Name });
        try
        {
            logOutput("Adding NServiceBus Performance Counters");
            await Task.Run(() =>
            {
                if (PerformanceCounterCategory.Exists(categoryName))
                {
                    PerformanceCounterCategory.Delete(categoryName);
                }
                var counterCreationCollection = new CounterCreationDataCollection(Counters.ToArray());
                PerformanceCounterCategory.Create(categoryName, "NServiceBus statistics", PerformanceCounterCategoryType.MultiInstance, counterCreationCollection);
                PerformanceCounter.CloseSharedResources(); // http://blog.dezfowler.com/2007/08/net-performance-counter-problems.html
            }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logError("NServiceBus Performance Counters install failed:");
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

    public int NestedActionCount => 1;
    public string Name => "NServiceBus Performance Counters";
    public string Description => "Optional Install";
    public string Status => InstallState == InstallState.Installed ? "Installed" : "Install Counters";
    public bool SelectedByDefault => false;
    public bool RebootRequired => false;
    const string categoryName = "NServiceBus";

    static List<CounterCreationData> Counters = new List<CounterCreationData>
    {
        new CounterCreationData("Critical Time", "Age of the oldest message in the queue.", PerformanceCounterType.NumberOfItems32),
        new CounterCreationData("SLA violation countdown","Seconds until the SLA for this endpoint is breached.",PerformanceCounterType.NumberOfItems32),
        new CounterCreationData("# of msgs successfully processed / sec", "The current number of messages processed successfully by the transport per second.",PerformanceCounterType.RateOfCountsPerSecond32),
        new CounterCreationData("# of msgs pulled from the input queue /sec", "The current number of messages pulled from the input queue by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32),
        new CounterCreationData("# of msgs failures / sec", "The current number of failed processed messages by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32)
    };
}
