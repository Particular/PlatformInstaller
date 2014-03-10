using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

internal class PlatformInstallerPSHostUI : PSHostUserInterface
{

    PowerShellRunner runner;

    public PlatformInstallerPSHostUI(PowerShellRunner runner)
    {
        this.runner = runner;
    }

    public override string ReadLine()
    {
        throw new NotImplementedException("ReadLine is not implemented");
    }

    public override SecureString ReadLineAsSecureString()
    {
        throw new NotImplementedException("ReadLineAsSecureString is not implemented");
    }

    public override void Write(string value)
    {
        runner.OutputDataReceived(new PowerShellOutputLine(value, PowerShellLineType.Output, false));
    }

    public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
        runner.OutputDataReceived(new PowerShellOutputLine(value, PowerShellLineType.Output, false));   
    }

    public override void WriteLine(string value)
    {
        runner.OutputDataReceived(new PowerShellOutputLine(value, PowerShellLineType.Output));
    }

    public override void WriteErrorLine(string value)
    {
        runner.OutputDataReceived(new PowerShellOutputLine(value, PowerShellLineType.Error));
    }

    public override void WriteDebugLine(string value)
    {
        runner.OutputDataReceived(new PowerShellOutputLine(value, PowerShellLineType.Debug));
    }

    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
        runner.OutputProgessReceived(record);
    }

    public override void WriteVerboseLine(string value)
    {
        runner.OutputDataReceived(new PowerShellOutputLine(value, PowerShellLineType.Verbose));
    }

    public override void WriteWarningLine(string value)
    {
        runner.OutputDataReceived(new PowerShellOutputLine(value, PowerShellLineType.Warning));
    }

    public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
    {
        throw new NotImplementedException("Prompt is not implemented.");
    }

    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
    {
        throw new NotImplementedException("PromptForCredential is not implemented.");
    }

    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
    {
        throw new NotImplementedException("PromptForCredential is not implemented.");
    }

    public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
    {
        throw new NotImplementedException("PromptForChoice is not implemented.");
    }

    private readonly PlatformInstallerPSHostRawUserInterface piRawUI = new PlatformInstallerPSHostRawUserInterface();

    public override PSHostRawUserInterface RawUI
    {
        get { return piRawUI; }
    }
}