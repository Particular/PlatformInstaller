using System.Linq;
using Autofac;
using NUnit.Framework;

[TestFixture]
public class AutofacExtensionsTests
{
    [Test]
    public void Verify()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<MySingleInstanceService>()
            .SingleInstance();
        builder.RegisterType<MyMultiInstanceService>();

        var singleInstanceRegistrations = builder.Build().GetSingleInstanceRegistrations().ToList();
        Assert.That(singleInstanceRegistrations, Has.Member(typeof(MySingleInstanceService)));
        Assert.That(singleInstanceRegistrations, Has.No.Member(typeof(MyMultiInstanceService)));
    }

    public class MyMultiInstanceService
    {
    }

    public class MySingleInstanceService
    {
    }
}