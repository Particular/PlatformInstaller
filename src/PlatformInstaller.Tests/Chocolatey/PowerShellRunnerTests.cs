//using System;
//using System.Linq;
//using System.Management.Automation;
//using NUnit.Framework;

//[TestFixture]
//public class PowerShellRunnerTests
//{

//    [Test]
//    public void VerifyOutputIsWritten()
//    {
//        var runAsync = new PowerShellRunner("write test");
//        var result = "";
//        runAsync.OutputDataReceived = x =>
//        {
//            result = x;
//        };
//        runAsync.Run().Wait();

//        Assert.AreEqual("test", result);
//    }

//    [Test]
//    public void VerifyErrorIsWritten()
//    {
//        var runAsync = new PowerShellRunner("Write-Error \"oops\"");
//        var result = "";
//        runAsync.OutputErrorReceived = x =>
//        {
//            result = x;
//        };
//        runAsync.Run().Wait();

//        Assert.AreEqual("oops", result);
//    }

//    [Test]
//    public void VerifyAnInvalidCommandThrowsCommandNotFoundException()
//    {
//        var runSync = new PowerShellRunner("thingdingding");
//        var task = runSync.Run();
//        var exception = Assert.Throws<AggregateException>(task.Wait);
//        Assert.IsInstanceOf<CommandNotFoundException>(exception.InnerExceptions.First());
//    }

//}