using NUnit.Framework;

[TestFixture]
public class RebootNeededViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void Show()
    {
        var model = GetModel();
        ViewTester.ShowView(model);
    }


    [Test]
    [RequiresSTA]
    [Explicit]
    public void Screenshot()
    {
        var model = GetModel();
        ViewTester.ScreenCapture(model);
    }

    static RebootNeededViewModel GetModel()
    {
        return new RebootNeededViewModel(new FakeEventAggregator());
    }
}
