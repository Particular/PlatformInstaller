using System;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
[Explicit]
public class ProcessRunnerTests
{

    [Test]
    public void VerifyOutputReceived()
    {
        var progressService = new ProgressService();
        var processRunner = new ProcessRunner(progressService);
        processRunner.RunProcess("ping", "localhost").Wait();
        ObjectApprover.VerifyWithJson(progressService.OutputText.ReplaceCaseless(Environment.MachineName, ""));
    }

    [Test]
    public void VerifyErrorReceived()
    {
        var progressService = new ProgressService();
        var processRunner = new ProcessRunner(progressService);
        processRunner.RunProcess("net", " use foo").Wait();
        ObjectApprover.VerifyWithJson(progressService.OutputText.ReplaceCaseless(Environment.MachineName, ""));
    }

}