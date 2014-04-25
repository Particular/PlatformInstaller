using System.Collections.Generic;

public class FakePackageDefinitionService : PackageDefinitionService
{
    public FakePackageDefinitionService() : base(null)
    {
    }

    public override List<InstallationDefinition> GetPackages()
    {
        return new List<InstallationDefinition>();
    }
}