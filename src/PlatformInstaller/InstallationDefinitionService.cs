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

        var definitions = new List<InstallationDefinition>
        {
            new InstallationDefinition
            {
                Installer = nsbPreRequisitesRunner,
            },
            new InstallationDefinition
            {
                Installer =  scRunner,
            },
            new InstallationDefinition
            {
                Installer = spRunner,
            },
            new InstallationDefinition
            {
                Installer = siRunner,
            },
            new InstallationDefinition
            {
                Installer = sm2013Runner,
            },  
            new InstallationDefinition
            {
                Installer = sm2012Runner,
            }  
        };

        return definitions;
    }
}