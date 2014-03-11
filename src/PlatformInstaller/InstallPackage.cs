using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformInstaller
{
    class InstallPackage : IPackage
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public IPackage Parent { get; set; }

        public bool Selected { get; set; }

        public IEnumerable<IPackage> Children { get; set; }
    }
}
