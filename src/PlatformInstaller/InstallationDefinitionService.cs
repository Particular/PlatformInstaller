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
                Name = nsbPreRequisitesRunner.Name,
                Image = "/Images/NSB.png",
                Installer = nsbPreRequisitesRunner,
                Disabled = nsbPreRequisitesRunner.Disabled ,
                ToolTip = nsbPreRequisitesRunner.ToolTip,
                Status = nsbPreRequisitesRunner.Status(),
                FeedOK = true,
                NoErrors = true,
            },
            new InstallationDefinition
            {
                Name = scRunner.Name,
                Image = "/Images/SC.png",
                Installer =  scRunner,
                Disabled = scRunner.Disabled,
                Status = scRunner.Status(),
                FeedOK = scRunner.HasReleaseInfo(),
                NoErrors = true
            },
            new InstallationDefinition
            {
                Name = "ServicePulse",
                Image = "/Images/SP.png",
                Installer = spRunner,
                Disabled =  spRunner.Disabled, 
                Status = spRunner.Status(),
                FeedOK = spRunner.HasReleaseInfo(),
                NoErrors = true
            },
            new InstallationDefinition
            {
                Name = siRunner.Name,
                Image = "/Images/SI.png",
                Installer = siRunner,
                Disabled = siRunner.Disabled, 
                Status = siRunner.Status(),
                FeedOK = siRunner.HasReleaseInfo(),
                NoErrors = true
            },
            new InstallationDefinition
            {
                Name = sm2013Runner.Name,
                Image = "/Images/SM2013.png",
                Installer = sm2013Runner,
                Disabled =  sm2013Runner.Disabled,
                Status = sm2013Runner.Status(),
                ToolTip =  sm2013Runner.ToolTip,
                FeedOK = sm2013Runner.HasReleaseInfo(),
                NoErrors = sm2013Runner.NoErrors
            },   
            new InstallationDefinition
            {
                Name = sm2012Runner.Name,
                Image = "/Images/SM2012.png",
                Installer = sm2012Runner,
                Disabled = sm2012Runner.Disabled,
                Status = sm2012Runner.Status(),
                ToolTip = sm2012Runner.ToolTip,
                FeedOK = sm2012Runner.HasReleaseInfo(),
                NoErrors = sm2012Runner.NoErrors
            }   
        };

        return definitions;
    }
}