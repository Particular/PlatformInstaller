using NUnit.Framework;

[TestFixture]
public class ReadableBytesConverterTests
{
    [Test]
    public void Verify()
    {
        Assert.AreEqual("0B", 0.ToBytesString());
        Assert.AreEqual("1.0B", 1.ToBytesString());
        Assert.AreEqual("500.0B", 500.ToBytesString());
        Assert.AreEqual("1023.0B", 1023.ToBytesString());
        Assert.AreEqual("1.0KB", 1024.ToBytesString());
        Assert.AreEqual("1.0KB", 1025.ToBytesString());
        Assert.AreEqual("1024.0KB", 1048575.ToBytesString());
        Assert.AreEqual("1.0MB", 1048576.ToBytesString());
        Assert.AreEqual("1.0MB", 1048577.ToBytesString());
        Assert.AreEqual("1024.0MB", 1073741823.ToBytesString());
        Assert.AreEqual("1.0GB", 1073741824.ToBytesString());
        Assert.AreEqual("1.0GB", 1073741825.ToBytesString());
    }
}