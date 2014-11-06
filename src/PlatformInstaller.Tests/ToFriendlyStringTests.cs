using System;
using System.Text.RegularExpressions;
using ApprovalTests;
using NUnit.Framework;


[TestFixture]
public class ToFriendlyStringTests
{
    [Test]
    public void ToFriendlyString()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory.ToLowerInvariant()
            .Replace(@"bin\debug", string.Empty)
            .Replace(@"bin\release", string.Empty);
        try
        {
            ThrowException1();
        }
        catch (Exception exception)
        {
            var friendlyString = exception.ToFriendlyString().ToLowerInvariant();
            friendlyString = friendlyString
                .Replace(currentDirectory, string.Empty);
            friendlyString = new Regex(":line.*").Replace(friendlyString, "");
            Approvals.Verify(friendlyString);
        }
    }
    void ThrowException1()
    {
        ThrowException2();
    }

    void ThrowException2()
    {
        throw new Exception("Foo");
    }
}