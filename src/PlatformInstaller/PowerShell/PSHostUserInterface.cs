using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using Anotar.Serilog;

public class PSHostUserInterface : System.Management.Automation.Host.PSHostUserInterface
{
    PSHostRawUserInterface piRawUI = new PSHostRawUserInterface();
    PowerShellRunner powerShellRunner;

    public PSHostUserInterface(PowerShellRunner powerShellRunner)
    {
        this.powerShellRunner = powerShellRunner;
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
        if (value == "\n" || value == "\r\n")
        {
            return;
        }
        WriteLine(value);
    }

    public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
        WriteLine(value);
    }

    public override void WriteLine(string value)
    {
        value = value.TrimEnd('\n', '\r');
        LogTo.Information(value);
        powerShellRunner.LogOutput(value);
    }

    public override void WriteErrorLine(string value)
    {
        LogTo.Error(value);
        powerShellRunner.LogError(value);
    }

    public override void WriteDebugLine(string value)
    {
        LogTo.Debug(value);
        powerShellRunner.LogOutput(value);
    }

    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
        LogTo.Debug(record.ToString());
        powerShellRunner.LogProgress(record);
    }

    public override void WriteVerboseLine(string value)
    {
        LogTo.Debug(value);
        powerShellRunner.LogOutput(value);
    }

    public override void WriteWarningLine(string value)
    {
        LogTo.Warning(value);
        powerShellRunner.LogWarning(value);
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

    public override System.Management.Automation.Host.PSHostRawUserInterface RawUI
    {
        get { return piRawUI; }
    }
}