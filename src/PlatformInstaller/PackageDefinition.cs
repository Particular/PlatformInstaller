using System;
using System.Threading.Tasks;
using PropertyChanged;

[ImplementPropertyChanged]
public class PackageDefinition
{

    public string Name;
    public string Image;
    public bool Selected;
    public Func<Task> InstallAction;
    public bool Installed;
}