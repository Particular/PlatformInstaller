using System.Collections.Generic;


public class InstallationDefinitionService
{
    ServiceControlInstallRunner scRunner;
    ServicePulseInstallRunner spRunner;
    ServiceInsightInstallRunner siRunner;
    ServiceMatrix2012InstallRunner sm2012Runner;
    ServiceMatrix2013InstallRunner sm2013Runner;
    NServiceBusPrerequisitesInstallRunner nsbPreRequisitesRunner;

    public InstallationDefinitionService(ServiceControlInstallRunner scRunner,
                         ServicePulseInstallRunner spRunner,
                         ServiceInsightInstallRunner siRunner,
                         ServiceMatrix2012InstallRunner sm2012Runner,
                         ServiceMatrix2013InstallRunner sm2013Runner,
                         NServiceBusPrerequisitesInstallRunner nsbPreRequisitesRunner)
    {
        this.scRunner = scRunner;
        this.siRunner = siRunner;
        this.spRunner = spRunner;
        this.sm2012Runner = sm2012Runner;
        this.sm2013Runner = sm2013Runner;
        this.nsbPreRequisitesRunner = nsbPreRequisitesRunner;
    }

    public virtual List<InstallationDefinition> GetPackages()
    {
        foreach (var runner in new IInstallRunner[] { scRunner, siRunner, spRunner, sm2012Runner, sm2013Runner})
        {
            runner.GetReleaseInfo();
        }

        var definitions = new List<InstallationDefinition>
        {
            new InstallationDefinition
            {
                Name = "NServiceBus Pre-requisites",
                Image = "/Images/NSB.png",
                Installer = nsbPreRequisitesRunner,
                Disabled = false,
                SelectedByDefault = true,
                ToolTip = "Configure MSMQ, DTC and install NServiceBus Performance Counters",
                Status = nsbPreRequisitesRunner.Status(),
                FeedOK = true,
                NoErrors = true,
            },
            new InstallationDefinition
            {
                Name = "ServiceControl",
                Image = "/Images/SC.png",
                Installer =  scRunner,
                Disabled = scRunner.LatestAvailableVersion() == scRunner.CurrentVersion(),
                SelectedByDefault = (scRunner.LatestAvailableVersion() != scRunner.CurrentVersion()),
                Status = scRunner.Status(),
                FeedOK = scRunner.HasReleaseInfo(),
                NoErrors = true
            },
            new InstallationDefinition
            {
                Name = "ServicePulse",
                Image = "/Images/SP.png",
                Installer = spRunner,
                Disabled =  (spRunner.LatestAvailableVersion() == spRunner.CurrentVersion()), 
                SelectedByDefault = (spRunner.LatestAvailableVersion() != spRunner.CurrentVersion()),
                Status = spRunner.Status(),
                FeedOK = spRunner.HasReleaseInfo(),
                NoErrors = true
            },
            new InstallationDefinition
            {
                Name = "ServiceInsight",
                Image = "/Images/SI.png",
                Installer = siRunner,
                Disabled =   (siRunner.LatestAvailableVersion() == siRunner.CurrentVersion()), 
                SelectedByDefault = (siRunner.LatestAvailableVersion() != siRunner.CurrentVersion()),
                Status = siRunner.Status(),
                FeedOK = siRunner.HasReleaseInfo(),
                NoErrors = true
            },
            new InstallationDefinition
            {
                Name = "ServiceMatrix for Visual Studio 2013",
                Image = "/Images/SM2013.png",
                Installer = sm2013Runner,
                Disabled = !VisualStudioDetecter.VS2013Installed | sm2013Runner.Installed(),
                SelectedByDefault = (VisualStudioDetecter.VS2013Installed && !sm2013Runner.Installed()),
                Status = sm2013Runner.Status(),
                ToolTip =  (!VisualStudioDetecter.VS2013Installed) ? "Requires Visual Studio 2013 Professional or higher" : null,
                FeedOK = sm2013Runner.HasReleaseInfo(),
                NoErrors = VisualStudioDetecter.VS2013Installed  && !sm2013Runner.Installed()
            },   
            new InstallationDefinition
            {
                Name = "ServiceMatrix for Visual Studio 2012",
                Image = "/Images/SM2012.png",
                Installer = sm2012Runner,
                Disabled = !VisualStudioDetecter.VS2012Installed | sm2012Runner.Installed(),
                SelectedByDefault = (VisualStudioDetecter.VS2012Installed & !sm2012Runner.Installed()),
                Status = sm2012Runner.Status(),
                ToolTip = (!VisualStudioDetecter.VS2012Installed) ? "Requires Visual Studio 2012 Professional or higher" : null,
                FeedOK = sm2012Runner.HasReleaseInfo(),
                NoErrors = VisualStudioDetecter.VS2012Installed  && !sm2012Runner.Installed()
            }   
        };

        return definitions;
    }
}