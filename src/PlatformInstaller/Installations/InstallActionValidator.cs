using System.Collections.Generic;
using System.Linq;

public class InstallActionValidator
{
    IEnumerable<IInstaller> installers;

    public InstallActionValidator(IEnumerable<IInstaller> installers)
    {
        this.installers = installers;
    }

    public bool ValidateInstallationProposal(IList<IInstaller> selected, out string[] problems)
    {
        problems = FindProblems(selected).ToArray();

        return !problems.Any();
    }

    IEnumerable<string> FindProblems(IList<IInstaller> selected)
    {
        var installed = installers.Where(x => x.InstallState != InstallState.NotInstalled).ToArray();

        var installingPulseWithoutControl = !installed.Contains<ServicePulseInstaller>()
                                            && !installed.Contains<ServiceControlInstaller>()
                                            && selected.Contains<ServicePulseInstaller>()
                                            && !selected.Contains<ServiceControlInstaller>();

        if (installingPulseWithoutControl)
        {
            yield return "You are installing ServicePulse without installing ServiceControl. ServicePulse requires a ServiceControl instance, either on this machine or elsewhere. If you intend to use ServicePulse with an existing ServiceControl instance, or one that you will create later, enter the appropriate connection details during ServicePulse installation.";
        }
    }
}
