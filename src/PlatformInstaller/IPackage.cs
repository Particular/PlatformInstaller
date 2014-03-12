using System.Collections.Generic;

public interface IPackage
{
    string Name { get; }
    string Image { get; } //may change

    bool Selected { get; set; }
    bool Automatic { get; }

    string Chocolatey { get; }

    IEnumerable<IPackage> Children { get; } //may make more sense to have dependencies than children
}