using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using Anotar.Serilog;

public class PlatformInstallerPSHostUI : PSHostUserInterface
{

    ProgressService progressService;

    public PlatformInstallerPSHostUI(ProgressService progressService)
    {
        this.progressService = progressService;
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
        LogTo.Information(value);
        progressService.OutputDataReceived(new LogEntry(value, LogEntryType.Output, false));
    }

    public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
        LogTo.Information(value);
        progressService.OutputDataReceived(new LogEntry(value, LogEntryType.Output, false));   
    }

    public override void WriteLine(string value)
    {
        LogTo.Information(value);
        progressService.OutputDataReceived(new LogEntry(value, LogEntryType.Output));
    }

    public override void WriteErrorLine(string value)
    {
        LogTo.Error(value);
        progressService.OutputDataReceived(new LogEntry(value, LogEntryType.Error));
    }

    public override void WriteDebugLine(string value)
    {
        LogTo.Debug(value);
        progressService.OutputDataReceived(new LogEntry(value, LogEntryType.Debug));
    }

    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
        LogTo.Debug(record.ToString());
        progressService.OutputProgressReceived(record);
    }

    public override void WriteVerboseLine(string value)
    {
        LogTo.Debug(value);
        progressService.OutputDataReceived(new LogEntry(value, LogEntryType.Verbose));
    }

    public override void WriteWarningLine(string value)
    {
        LogTo.Warning(value);
        progressService.OutputDataReceived(new LogEntry(value, LogEntryType.Warning));
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