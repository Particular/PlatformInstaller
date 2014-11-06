using NUnit.Framework;

[TestFixture]
public class ReadableBytesConverterTests
{
    [Test]
    public void Verify()
    {
        Assert.AreEqual("0B", 0.ToBytesString());
        Assert.AreEqual("1B", 1.ToBytesString());
        Assert.AreEqual("500B", 500.ToBytesString());
        Assert.AreEqual("1023B", 1023.ToBytesString());
        Assert.AreEqual("1KB", 1024.ToBytesString());
        Assert.AreEqual("1KB", 1025.ToBytesString());
        Assert.AreEqual("1024KB", 1048575.ToBytesString());
        Assert.AreEqual("1MB", 1048576.ToBytesString());
        Assert.AreEqual("1MB", 1048577.ToBytesString());
        Assert.AreEqual("1024MB", 1073741823.ToBytesString());
        Assert.AreEqual("1GB", 1073741824.ToBytesString());
        Assert.AreEqual("1GB", 1073741825.ToBytesString());
    }
}