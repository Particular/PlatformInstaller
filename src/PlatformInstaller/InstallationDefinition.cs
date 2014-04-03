using System.Collections.Generic;

public class InstallationDefinition 
{
    public string Name;
    public string ToolTip;
    public string Image;
    public List<PackageDefinition> PackageDefinitions;
    public bool Disabled;
    public bool SelectedByDefault;
    public string Status;

    public InstallationDefinition()
    {
        ToolTip = Name;
    }
}