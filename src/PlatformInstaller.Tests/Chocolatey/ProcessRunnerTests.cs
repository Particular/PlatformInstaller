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
        var processRunner = new ProcessRunner();
        string output = null;
        processRunner.RunProcess("ping", "localhost", s => output += s, s => output += s).Wait();
        ObjectApprover.VerifyWithJson(output.ReplaceCaseless(Environment.MachineName, ""));
    }

    [Test]
    public void VerifyErrorReceived()
    {
        var processRunner = new ProcessRunner();
        string output = null;
        processRunner.RunProcess("net", " use foo", s => output += s, s => output += s).Wait();
        ObjectApprover.VerifyWithJson(output.ReplaceCaseless(Environment.MachineName, ""));
    }

}