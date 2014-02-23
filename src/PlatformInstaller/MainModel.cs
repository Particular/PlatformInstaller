using System;
using PropertyChanged;

[ImplementPropertyChanged]
public class MainViewModel
{
    public string OutputText { get; set; }
    public bool IsInstalling;

    public void InstallServiceMatrix()
    {
        
    }
    public void InstallServiceInsight()
    {
        
    }
    public async void InstallMsmq()
    {
        OutputText = "";
        var packageInstaller = new PackageManager("Pester")
        {
            OutputDataReceived = s =>
            {
                OutputText += s+ Environment.NewLine;
            }
        };
        await packageInstaller.Install();
    }
    public void InstallDTC()
    {
        
    }
    public void InstallServiceControl()
    {
        
    }
}