using NUnit.Framework;

[TestFixture]
public class PendingRestartTests
{
    [Test]
    [Explicit]
    public void AddPendingRestart()
    {
        new PendingRestart().AddPendingRestart();
    }
    [Test]
    [Explicit]
    public void RemovePendingRestart()
    {
        new PendingRestart().RemovePendingRestart();
    }
}