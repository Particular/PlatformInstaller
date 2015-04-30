using System;
using NuGet;

public interface IInstallRunner
{
    SemanticVersion CurrentVersion();
    SemanticVersion LatestAvailableVersion();
    void Execute(Action<string> logOutput, Action<string> logError);
    bool Installed();
    int NestedActionCount { get; }
    string Status();
    void GetReleaseInfo();
    bool HasReleaseInfo();
    int InstallationResult { get;  }

}