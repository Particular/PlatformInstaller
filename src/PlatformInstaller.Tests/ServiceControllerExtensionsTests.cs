using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using NUnit.Framework;

[TestFixture]
public class ServiceControllerExtensionsTests
{
    [Test]
    [Explicit]
    public async Task WaitForStatusAsync()
    {
        var serviceController = new ServiceController("MSMQ");
        if (serviceController.Status != ServiceControllerStatus.Running)
        {
            serviceController.Start();
        }
        serviceController.WaitForStatus(ServiceControllerStatus.Running);

        var task = serviceController.WaitForStatusAsync(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));

        serviceController.Stop();
        await task.ConfigureAwait(false);
        serviceController.Refresh();
        Assert.AreEqual(ServiceControllerStatus.Stopped, serviceController.Status);
    }
}