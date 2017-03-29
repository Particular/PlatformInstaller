using System.Diagnostics;
using Caliburn.Micro;

public class RebootMachine :
    IHandle<RebootMachineCommand>
{

    public void Handle(RebootMachineCommand message)
    {
       Process.Start("shutdown", "/g /c \"You have 20 seconds to save work and close applications\"");
    }
}