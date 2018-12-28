﻿using System.Collections.Generic;
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
            yield return "The installation of ServicePulse requires a ServiceControl instance. If you’re installing ServiceControl on this machine or another one, make sure the connection details entered during the installation of ServicePulse are the same that will be used for ServiceControl.";
        }
    }
}