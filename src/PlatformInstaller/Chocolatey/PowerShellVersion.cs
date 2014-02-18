public class PowerShellVersion
{
    public void Initialise()
    {
        //TODO: test on system with no PS
        IsInstalled = true;

        string output = null;
        var runner = new PowerShellRunner("$PSVersionTable.PSVersion")
        {
            OutputDataReceived = x =>
            {
                output = x;
            }
        };
        runner.Run().Wait();
        Version = System.Version.Parse(output).Major;
    }

    public bool IsInstalled;
    public int Version;
}