using System;

public interface IInstallRunner
{
    Version CurrentVersion();
    Version LatestAvailableVersion();
    void Execute(Action<string> logOutput, Action<string> logError);
    bool Installed();
    int NestedActionCount { get; }
    string Status();
    void GetReleaseInfo();
    bool HasReleaseInfo();
    int InstallationResult { get;  }

}