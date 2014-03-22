using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Autofac;

public class PackageDefinition : INotifyPropertyChanged
{
    private PackageManager packageManager;

    public PackageDefinition()
    {
        packageManager = ContainerFactory.Container.Resolve<PackageManager>();
        Dependencies = new List<PackageDefinition>();

        Dependencies.BindActionToPropChanged(() =>
        {
            selected = Dependencies.Any(d => d.Selected);
        }, "Selected");

    }

    public string Name;
    public string Image;
    public string ImageUrl { get { return "pack://application:,,,/PlatformInstaller;component" + Image; } }
    public string ChocolateyPackage;

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