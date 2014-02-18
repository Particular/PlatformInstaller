using System;
using System.Threading;
using NUnit.Framework;

[TestFixture]
public class PowerShellRunnerTests
{

    [Test]
    public void IfCanRunCommandAndReturnOutput()
    {
        var runAsync = new PowerShellRunner();
        var result = "";
        var waitHandle = new AutoResetEvent(false);
        runAsync.OutputChanged += x =>
        {
            result = x;
            waitHandle.Set();
        };
        runAsync.Run("write test");

        waitHandle.WaitForTimeout(TimeSpan.FromSeconds(10));

        Assert.AreEqual("test\r\n", result);
    }

    [Test]
    public void IfCanRunCommandAndRaiseEventRunFinished()
    {
        var runSync = new PowerShellRunner();
        var result = 0;
        var waitHandle = new AutoResetEvent(false);
        runSync.RunFinished += () =>
        {
            result = 1;
            waitHandle.Set();
        };
        runSync.Run("write test");
        waitHandle.WaitForTimeout(TimeSpan.FromSeconds(10));
        Assert.AreEqual(1, result);
    }

    [Test]
    public void IfCanRunCommandWithNoOutputAndEventRunFinishedIsRaised()
    {
        var runSync = new PowerShellRunner();
        var result = 0;
        var waitHandle = new AutoResetEvent(false);
        runSync.RunFinished += () =>
        {
            result = 1;
            waitHandle.Set();
        };
        runSync.Run("write");
        waitHandle.WaitForTimeout(TimeSpan.FromSeconds(10));
        Assert.AreEqual(1, result);
    }

    [Test]
    public void IfCanRunCommandWithNoOutputAndEventOutputChangedIsRaised()
    {
        var runSync = new PowerShellRunner();
        var result = "";
        var waitHandle = new AutoResetEvent(false);
        runSync.OutputChanged += x =>
        {
            result = x;
            waitHandle.Set();
        };
        runSync.Run("write");
        waitHandle.WaitForTimeout(TimeSpan.FromSeconds(10));
        Assert.AreEqual("No output", result);
    }

    [Test]
    public void IfCanRunWrongCommandAndEventRunFinishedIsRaised()
    {
        var runSync = new PowerShellRunner();
        var result = 0;
        var waitHandle = new AutoResetEvent(false);
        runSync.RunFinished += () =>
        {
            result = 1;
            waitHandle.Set();
        };
        runSync.Run("thingdingding");
        waitHandle.WaitForTimeout(TimeSpan.FromSeconds(10));
        Assert.AreEqual(1, result);
    }

    [Test]
    public void IfCanRunWrongCommandWithNoOutputAndEventOutputChangedIsRaised()
    {
        var runSync = new PowerShellRunner();
        var result = "";
        var waitHandle = new AutoResetEvent(false);
        runSync.OutputChanged += x =>
        {
            result = x;
            waitHandle.Set();
        };
        runSync.Run("thingdingding");
        waitHandle.WaitForTimeout(TimeSpan.FromSeconds(10));
        Assert.AreEqual("No output", result);
    }
}