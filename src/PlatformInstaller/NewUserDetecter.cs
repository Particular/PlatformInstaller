namespace PlatformInstaller.Tests
{
    using System.Linq;
    using Microsoft.Win32;

    public class NewUserDetecter
    {
        readonly RegistryKey regKey;

        public static NewUserDetecter Current
        {
            get
            {
                return new NewUserDetecter(Registry.CurrentUser);
            }
        }

        public NewUserDetecter(RegistryKey regKey)
        {
            this.regKey = regKey;
        }

        public bool IsNewUser()
        {
            return regKey.GetSubKeyNames().All(subKey => 
                subKey != "NServiceBus" &&
                subKey != "ParticularSoftware");
        }
    }
}