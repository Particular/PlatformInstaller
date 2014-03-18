using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Autofac;
using PlatformInstaller;

public class PackageDefinition : INotifyPropertyChanged
{
    private PackageManager packageManager;

    public PackageDefinition()
    {
        this.packageManager = ContainerFactory.Container.Resolve<PackageManager>();
        Dependencies = new List<PackageDefinition>();

        Dependencies.BindActionToPropChanged(() =>
        {
            selected = Dependencies.Any(d => d.Selected);
        }, "Selected");

    }

    public string Name { get; set; }
    public string Image { get; set; }
    public string ChocolateyPackage { get; set; }

    private bool selected;
    public bool Selected 
    { 
        get
        {
            return selected;
        }
        set
        {
            selected = value;
            foreach (var package in Dependencies) package.selected = value;
        }
    }
    
    
    public event PropertyChangedEventHandler PropertyChanged;

    public IEnumerable<PackageDefinition> Dependencies { get; set; }

    public bool Installed
    {
        get
        {
            return packageManager.IsInstalled(ChocolateyPackage);
        }
    }
}