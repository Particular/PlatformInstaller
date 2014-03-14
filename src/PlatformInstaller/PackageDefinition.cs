using System;
using System.ComponentModel;
using System.Threading.Tasks;

public class PackageDefinition : INotifyPropertyChanged
{

    public string Name;
    public string Image;
    public bool Selected;
    public Func<Task> InstallAction;
    public bool Installed;
    public event PropertyChangedEventHandler PropertyChanged;
}