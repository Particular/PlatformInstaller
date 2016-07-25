using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Caliburn.Micro;

public class MsmqInstaller : IInstaller
{
    IEventAggregator eventAggregator;
    private List<Dism.Feature> MSMQFeatures;

    public MsmqInstaller(IEventAggregator eventAggregator)
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
        List<Dism.Feature> features;
        if (!Dism.TryGetFeatures(out features))
        {
            throw new Exception("Failed to call out to DISM. This installer supports Window 7 SP1 and above on workstations and Windows 2008 R2 and above on servers");
        }
        MSMQFeatures = features.Where(p => p.Name.StartsWith("MSMQ")).ToList();
        var msmqServer = MSMQFeatures.Single(p => p.Name.Equals("MSMQ-Server"));
        InstallState = msmqServer.State == Dism.FeatureState.Enabled ? InstallState.Installed : InstallState.NotInstalled;
    }

    public async Task Execute(Action<string> output, Action<string> logError)
    {
        try
        {
            eventAggregator.PublishOnUIThread(new NestedInstallProgressEvent
            {
                Name = Name
            });
            await Task.Run(async () =>
            {
                try
                {
                    output("Started MSMQ Install");

                    var unsupportedFeatureNames = new[]
                    {
                        "MSMQ-ADIntegration",
                        "MSMQ-DCOMProxy",
                        "MSMQ-Multicast",
                        "MSMQ-RoutingServer",
                        "MSMQ-Triggers",
                    };

                    if (MSMQFeatures.Any(p => p.State == Dism.FeatureState.EnablePending || p.State == Dism.FeatureState.DisablePending))
                    {
                        await eventAggregator.PublishOnUIThreadAsync(new RebootRequiredEvent { Reason = "Windows requires a restart to complete a pending add or removal of an MSMQ component" });
                        RebootRequired = true;
                        return;
                    }

                    // MSMQ-Container is not found on all OSes. Where it exists it's a pre-req for MSMQ-Server
                    // On Win7 DISM does not have the /All command line switch which automatically enables pre-reqs like this so we do it explicitly
                    var msmqCoreContainer = MSMQFeatures.SingleOrDefault(p => p.Name.Equals("MSMQ-Container"));
                    if (msmqCoreContainer != null)
                    {

                        if (msmqCoreContainer.State != Dism.FeatureState.Enabled && msmqCoreContainer.State != Dism.FeatureState.EnablePending)
                        {
                            if (msmqCoreContainer.EnableFeature())
                            {
                                output($"Enabling {msmqCoreContainer.DisplayName}");
                            }
                            else
                            {
                                throw new Exception($"Failed to enable '{msmqCoreContainer.DisplayName}' via Windows DISM command line");
                            }
                        }
                    }
                    var msmqServerFeature = MSMQFeatures.Single(p => p.Name.Equals("MSMQ-Server"));
                    if (msmqServerFeature.State != Dism.FeatureState.Enabled && msmqServerFeature.State != Dism.FeatureState.EnablePending)
                    {
                        if (msmqServerFeature.EnableFeature())
                        {
                            output($"Enabled '{msmqServerFeature.DisplayName}' via Windows DISM command line.");
                        }
                        else
                        {
                            throw new Exception($"Failed to enable '{msmqServerFeature.DisplayName}' via Windows DISM command line");
                        }
                    }
                    var unsupportedFeatures = MSMQFeatures.Where(p => unsupportedFeatureNames.Contains(p.Name)).ToList();
                    var unsupportedFeaturesInstalled = unsupportedFeatures.Where(p => p.State == Dism.FeatureState.EnablePending || p.State == Dism.FeatureState.Enabled).ToList();
                    if (unsupportedFeaturesInstalled.Any())
                    {
                        var featureList = string.Join("\r\n", unsupportedFeaturesInstalled.Select(p => $" - {p.DisplayName}"));
                        throw new Exception($"NServiceBus does not support the following Windows feature(s):\r\n{featureList}\r\nPlease remove them via Windows Add/Remove Features or DISM.exe");
                    }

                    try
                    {
                        using (var controller = new ServiceController("MSMQ"))
                        {
                            if (IsStopped(controller))
                            {
                                output("Starting MSMQ Service");
                                await controller.ChangeServiceStatus(ServiceControllerStatus.Running, controller.Start).ConfigureAwait(false);
                                output("MSMQ : OK");
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        throw new Exception("Failed to start MSMQ");
                    }

                }
                catch (Exception ex)
                {
                    logError($"{Name} failed:");
                    logError($"{ex}");
                }
            }).ConfigureAwait(false);
        }
        finally
        {
            eventAggregator.PublishOnUIThread(new NestedInstallCompleteEvent());
        }
    }


    static bool IsStopped(ServiceController controller)
    {
        return controller.Status == ServiceControllerStatus.Stopped || controller.Status == ServiceControllerStatus.StopPending;
    }

    public IEnumerable<AfterInstallAction> GetAfterInstallActions()
    {
        yield break;
    }

    public IEnumerable<DocumentationLink> GetDocumentationLinks()
    {
        yield break;
    }

    public int NestedActionCount => 2;
    public string Name => "Configure Microsoft Message Queuing";
    public string Description => "Optional - Required for MSMQ Transport Only";
    public string Status => InstallState == InstallState.Installed ? "Installed" : "Install MSMQ";
    public bool SelectedByDefault => false;
    public bool RebootRequired { get; private set; }
}
