using System.Diagnostics;
using Caliburn.Micro;

public class RebootMachine :
    IHandle<RebootMachineEvent>
{

    public void Handle(RebootMachineEvent message)
    {
       Process.Start("shutdown", "/g /c \"You have 20 seconds to save work and close applications\"");
    }
}