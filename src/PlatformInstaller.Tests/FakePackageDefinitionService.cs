using System.Collections.Generic;

public class FakePackageDefinitionService : PackageDefinitionService
{
    public FakePackageDefinitionService(PackageManager packageManager) : base(packageManager)
    {
    }

    public override List<PackageDefinition> GetPackages()
    {
        return new List<PackageDefinition>
        {
            new PackageDefinition
            {
                Name = "Service Matrix",
                Image = "/Images/SM.png",
                Installed =  true
            }};
    }
}