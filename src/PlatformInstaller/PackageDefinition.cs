using System;
using System.Threading.Tasks;
using PropertyChanged;

[ImplementPropertyChanged]
public class PackageDefinition
{

    public string Name;
    public string Image;
    public bool Selected = true;
    public bool Automatic { get; set; }
    public bool Enabled = true;
    public Func<Task> Install;

}