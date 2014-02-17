using System;
using System.Threading;
using NUnit.Framework;

[TestFixture]
public class PowerShellRunnerTests
{
    [Test]
    public void IfCanRunCommand()
    {
        var runAsync = new PowerShellRunner();
        runAsync.Run("write test");
    }

    [Test]
    public void IfCanRunCommandAndReturnOutput()
    {
        var runAsync = new PowerShellRunner();
        var result = "";
        var waitHandle = new AutoResetEvent(false);
        runAsync.OutputChanged += (x) =>
        {
            result = x;
            waitHandle.Set();
        };
        runAsync.Run("write test");

        waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(10));

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
        waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(10));
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
        waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(10));
        Assert.AreEqual(1, result);
    }

    [Test]
    public void IfCanRunCommandWithNoOutputAndEventOutputChangedIsRaised()
    {
        var runSync = new PowerShellRunner();
        var result = "";
        var waitHandle = new AutoResetEvent(false);
        runSync.OutputChanged += (x) =>
        {
            result = x;
            waitHandle.Set();
        };
        runSync.Run("write");
        waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(10));
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
        waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(10));
        Assert.AreEqual(1, result);
    }

    [Test]
    public void IfCanRunWrongCommandWithNoOutputAndEventOutputChangedIsRaised()
    {
        var runSync = new PowerShellRunner();
        var result = "";
        var waitHandle = new AutoResetEvent(false);
        runSync.OutputChanged += (x) =>
        {
            result = x;
            waitHandle.Set();
        };
        runSync.Run("thingdingding");
        waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(10));
        Assert.AreEqual("No output", result);
    }
}