using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformInstaller
{
    class InstallPackage : IPackage
    {
        public InstallPackage()
        {
            Children = new List<IPackage>();
        }

        public string Name { get; set; }

        public string Image { get; set; }

        public bool Selected { get; set; }

        public bool Automatic { get; set; }

        public string Chocolatey { get; set; }

        public IEnumerable<IPackage> Children { get; set; }
    }
}
