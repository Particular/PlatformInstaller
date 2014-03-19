using Caliburn.Micro;
using NUnit.Framework;

[TestFixture]
public class LicenseAgreementViewTests
{
    [Test]
    [Explicit]
    [RequiresSTA]
    public void ShowDialog()
    {

        ViewLocator.LocateForModel = (o, dependencyObject, arg3) => new LicenseAgreementView();
        new WindowManager().ShowDialog(new LicenseAgreementViewModel(new FakeEvenAggregator()));
    }

}

