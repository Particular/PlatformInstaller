using System;
using System.Threading.Tasks;
using PropertyChanged;

namespace PlatformInstaller
{
    [ImplementPropertyChanged]
    public class AppViewModel
    {
        public string OutputText { get; set; }
        public bool CanInstall = true;

        public async void InstallServiceInsight()
        {
            await InstallPackage("ServiceInsight");
        }

        public async void InstallServiceControl()
        {
            await InstallPackage("ServiceControl");
        }

        public async void InstallServicePulse()
        {
            await InstallPackage("ServicePulse");
        }

        public async void InstallServiceMatrix()
        {
            await InstallPackage("ServiceMatrix");
        }

        public async void InstallMsmq()
        {
            await InstallPackage("NServicebus.Msmq.install");
        }

        public async void InstallDtc()
        {
            await InstallPackage("NServicebus.Dtc.install");
        }
        public async void InstallPerfCounters()
        {
            await InstallPackage("NServicebus.PerfCounters.install");
        }

        public async void InstallRavenDb()
        {
            await InstallPackage("RavenDB");
        }

        async Task InstallPackage(string packageName)
        {
            OutputText = "";
            CanInstall = false;
            var packageInstaller = new PackageManager(packageName)
            {
                OutputDataReceived = s => { OutputText += s + Environment.NewLine; }
            };
            await packageInstaller.Install();
            CanInstall = true;
        }

    }
}