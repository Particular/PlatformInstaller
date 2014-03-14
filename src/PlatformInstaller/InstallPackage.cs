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

    public bool Selected;

    public bool Automatic { get; set; }

    private bool enabled;
    public bool Enabled
    {
        get
        {
            return enabled;
        }
        set
        {
            enabled = value;
            EnableDisableChildren(value);
        }
    }

    private void EnableDisableChildren(bool value)
    {
        foreach (var package in Children) package.Enabled = value;
    }

    public string Chocolatey { get; set; }

    public List<InstallPackage> Children { get; set; }
}
