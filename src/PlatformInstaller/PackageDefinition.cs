using System;
using System.Threading.Tasks;

public class PackageDefinition 
{
    public string Name;
    public string Image;
    public Func<Task> InstallAction ;
    public Func<bool> IsInstalledAction = () => false;
}