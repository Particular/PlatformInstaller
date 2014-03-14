using PropertyChanged;
using System.Collections.Generic;

[ImplementPropertyChanged]
public class InstallPackage
{
    public InstallPackage()
    {
        Enabled = true;
        Selected = true;
    }

    public string Name;

    public string Image;
    bool selected;

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            EnableDisableChildren(selected);
        }
    }


    public bool Automatic { get; set; }

    bool enabled;

    public bool Enabled
    {
        get { return enabled; }
        set
        {
            enabled = value;
            EnableDisableChildren(value);
        }
    }

    void EnableDisableChildren(bool value)
    {
        foreach (var package in Children) package.Enabled = value;
    }

    public string Chocolatey { get; set; }

    public List<InstallPackage> Children = new List<InstallPackage>();
}