using System;
using System.Collections.Generic;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class ProcessRunnerTests
{

    [Test]
    public void VerifyOutputReceived()
    {
        var outputList =new List<string>();
        var processRunner = new ProcessRunner("ping", "localhost")
        {
            OutputDataReceived = x => outputList.Add(x.ReplaceCaseless(Environment.MachineName, ""))
        };
        processRunner.RunProcessAsync().Wait();
        ObjectApprover.VerifyWithJson(outputList);
    }

    [Test]
    public void VerifyErrorReceived()
    {
        var errorList = new List<string>();
        var processRunner = new ProcessRunner("net", " use foo")
        {
            ErrorDataReceived = x => errorList.Add(x)
        };
        processRunner.RunProcessAsync().Wait();
        ObjectApprover.VerifyWithJson(errorList);
    }

}