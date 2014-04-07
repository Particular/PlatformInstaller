using NUnit.Framework;

[TestFixture]
public class LoggingTests
{
    [Test]
    [Explicit]
    public void OpenDirectory()
    {
        Logging.OpenLogDirectory();
    }
}