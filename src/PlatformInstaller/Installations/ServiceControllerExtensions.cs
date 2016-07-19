using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using TimeoutException = System.ServiceProcess.TimeoutException;

public static class ServiceControllerExtensions
{
    public static async Task WaitForStatusAsync(this ServiceController controller, ServiceControllerStatus desiredStatus, TimeSpan timeout)
    {
        var utcNow = DateTime.UtcNow;
        controller.Refresh();
        while (controller.Status != desiredStatus)
        {
            if (DateTime.UtcNow - utcNow > timeout)
            {
                throw new TimeoutException($"Failed to wait for '{controller.ServiceName}' to change status to '{desiredStatus}'.");
            }
            await Task.Delay(250)
                .ConfigureAwait(false);
            controller.Refresh();
        }
    }

}