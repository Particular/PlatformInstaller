using NUnit.Framework;

[TestFixture]
public class PendingRestartTests
{
    [Test]
    [Explicit]
    public void AddPendingRestart()
    {
        new PendingRestartAndResume().AddPendingRestart();
    }
    [Test]
    [Explicit]
    public void RemovePendingRestart()
    {
        new PendingRestartAndResume().RemovePendingRestart();
    }
}