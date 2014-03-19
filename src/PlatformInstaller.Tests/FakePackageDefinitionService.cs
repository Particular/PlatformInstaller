using System.Collections.Generic;

public class FakePackageDefinitionService : PackageDefinitionService
{

    public override List<PackageDefinition> GetPackages()
    {
        return new List<PackageDefinition>
        {
            new PackageDefinition
            {
                Name = "Service Matrix",
                Image = "/Images/SM.png"
            }};
    }
}