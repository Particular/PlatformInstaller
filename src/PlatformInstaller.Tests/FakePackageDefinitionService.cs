using System.Collections.Generic;

public class FakeInstallationDefinitionService : InstallationDefinitionService
{
    public FakeInstallationDefinitionService() : base(null,null,null,null,null,null)
    {
    }

    public override List<InstallationDefinition> GetPackages()
    {
        return new List<InstallationDefinition>();
    }
}