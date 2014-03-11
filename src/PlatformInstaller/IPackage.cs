using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformInstaller
{
    public interface IPackage
    {
        string Name { get; }
        string Image { get; } //may change
        string Description { get; }

        bool Selected { get; set; }

        IPackage Parent { get; }
        IEnumerable<IPackage> Children { get; } //may make more sense to have dependencies than children
    }
}
