using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IInstaller
{
    Version CurrentVersion();
    Version LatestAvailableVersion();
    void Init();
    Task Execute(Action<string> logOutput, Action<string> logError);
    IEnumerable<AfterInstallAction> GetAfterInstallActions();
    IEnumerable<DocumentationLink> GetDocumentationLinks();

    int NestedActionCount { get; }

    string Name { get; }
    string ImageName { get; }
    string Description { get; }

    bool SelectedByDefault { get; }
    InstallState InstallState { get; }
    string Status { get; }
}

public enum InstallState
{
    NotInstalled,
    Installed,
    UpgradeAvailable
}

public class DocumentationLink
{
    public string Text { get; set; }
    public string Url { get; set; }
}