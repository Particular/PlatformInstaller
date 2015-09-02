using System;
using System.Threading.Tasks;

public interface IInstallRunner
{
    Version CurrentVersion();
    Version LatestAvailableVersion();
    Task Execute(Action<string> logOutput, Action<string> logError);
    bool Installed();
    int NestedActionCount { get; }
    string Name { get; }
    string Status();
    void GetReleaseInfo();
    bool HasReleaseInfo();
    int InstallationResult { get;  }
    string ToolTip { get; }
    bool SelectedByDefault { get; }
}