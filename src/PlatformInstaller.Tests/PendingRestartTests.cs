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
}