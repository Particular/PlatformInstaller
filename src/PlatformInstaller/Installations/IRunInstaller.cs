using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IInstaller
{
    Version CurrentVersion();
    Version LatestAvailableVersion();
    void Init();
    Task Execute(Action<string> logOutput, Action<string> logError);
    bool Installed();
    IEnumerable<AfterInstallAction> GetAfterInstallActions();
    IEnumerable<DocumentationLink> GetDocumentationLinks();
    int NestedActionCount { get; }
    string Name { get; }
    string Status { get; }
    bool Enabled { get; }
    string ToolTip { get; }
    bool SelectedByDefault { get; }
}

public class DocumentationLink
{
    public string Text { get; set; }
    public string Url { get; set; }
}