using System.Collections.Generic;

public class InstallPackage 
{
    public InstallPackage()
    {
        Children = new List<InstallPackage>();
    }

    public string Name { get; set; }

    public string Image { get; set; }

    public bool Selected { get; set; }

    public bool Automatic { get; set; }

    public string Chocolatey { get; set; }

    public List<InstallPackage> Children { get; set; }
}
