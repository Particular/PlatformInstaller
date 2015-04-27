using System.Collections.Generic;
using PlatformInstaller.Installations;

public class InstallationDefinitionService
{
    ServiceControlInstallRunner scRunner;
    ServicePulseInstallRunner spRunner;
    ServiceInsightInstallRunner siRunner;
    ServiceMatrix2012InstallRunner sm2012Runner;
    ServiceMatrix2013InstallRunner sm2013Runner;
    NServiceBusPrequisitiesInstallRunner nsbPreRequisitesRunner;

    public InstallationDefinitionService(ServiceControlInstallRunner scRunner,
                         ServicePulseInstallRunner spRunner,
                         ServiceInsightInstallRunner siRunner,
                         ServiceMatrix2012InstallRunner sm2012Runner,
                         ServiceMatrix2013InstallRunner sm2013Runner,
                         NServiceBusPrequisitiesInstallRunner nsbPreRequisitesRunner)
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

        return new List<InstallationDefinition>
        {
            new InstallationDefinition
            {
                SortOrder = 10,
                Name = "NServiceBus Pre-requisites",
                Image = "/Images/NSB.png",
                Installer = nsbPreRequisitesRunner,
                Disabled = false,
                SelectedByDefault = true,
                ToolTip = "Configure MSMQ, DTC and install NServiceBus Performance Counters",
                Status = nsbPreRequisitesRunner.Status()
            },
            new InstallationDefinition
            {
                SortOrder = 50,
                Name = "ServiceControl",
                Image = "/Images/SC.png",
                Installer =  scRunner,
                Disabled =  !scRunner.HasReleaseInfo() | (scRunner.LatestAvailableVersion() == scRunner.CurrentVersion()),
                ToolTip =   !scRunner.HasReleaseInfo() ? "Could not retrieve latest release information" : null,
                SelectedByDefault = (scRunner.LatestAvailableVersion() != scRunner.CurrentVersion()),
                Status = scRunner.Status()
            },
            new InstallationDefinition
            {
                SortOrder = 40,
                Name = "ServicePulse",
                Image = "/Images/SP.png",
                Installer = spRunner,
                Disabled =  !spRunner.HasReleaseInfo() | (spRunner.LatestAvailableVersion() == spRunner.CurrentVersion()), 
                ToolTip =   !spRunner.HasReleaseInfo() ? "Could not retrieve latest release information" : null,
                SelectedByDefault = (spRunner.LatestAvailableVersion() != spRunner.CurrentVersion()),
                Status = spRunner.Status()
            },
            new InstallationDefinition
            {
                SortOrder = 30,
                Name = "ServiceInsight",
                Image = "/Images/SI.png",
                Installer = siRunner,
                Disabled =  !siRunner.HasReleaseInfo() |  (siRunner.LatestAvailableVersion() == siRunner.CurrentVersion()), 
                ToolTip =   !siRunner.HasReleaseInfo() ? "Could not retrieve latest release information" : null,
                SelectedByDefault = (siRunner.LatestAvailableVersion() != siRunner.CurrentVersion()),
                Status = siRunner.Status()
            },
            new InstallationDefinition
            {
                SortOrder = 20,
                Name = "ServiceMatrix for Visual Studio 2013",
                Image = "/Images/SM2013.png",
                Installer = sm2013Runner,
                Disabled = !VisualStudioDetecter.VS2013Installed | sm2013Runner.Installed() | !sm2013Runner.HasReleaseInfo() ,
                SelectedByDefault = (VisualStudioDetecter.VS2013Installed &&!sm2013Runner.Installed()) ,
                Status = sm2013Runner.Status(),
                ToolTip =  !sm2013Runner.HasReleaseInfo() ? "Could not retrieve latest release information" 
                            : (!VisualStudioDetecter.VS2013Installed) ? "Requires Visual Studio 2013 Professional or higher" : null
            },   
            new InstallationDefinition
            {
                SortOrder = 20,
                Name = "ServiceMatrix for Visual Studio 2012",
                Image = "/Images/SM2012.png",
                Installer = sm2012Runner,
                Disabled = !VisualStudioDetecter.VS2012Installed | sm2012Runner.Installed() | !sm2013Runner.HasReleaseInfo(),
                SelectedByDefault = (VisualStudioDetecter.VS2012Installed & !sm2012Runner.Installed()),
                Status = sm2012Runner.Status(),
                ToolTip = !sm2012Runner.HasReleaseInfo() ? "Could not retrieve latest release information" :
                        (!VisualStudioDetecter.VS2012Installed) ? "Requires Visual Studio 2012 Professional or higher" : null
            }   
        };
    }
}