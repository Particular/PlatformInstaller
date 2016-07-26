using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class RegistryFindTests
{
    [Test]
    public void FindInstalledProducts()
    {
        var installedProducts = RegistryFind.FindInstalledProducts().ToList();
        Assert.IsNotEmpty(installedProducts);
        foreach (var product in installedProducts)
        {
            Debug.WriteLine($"{product.ProductName} {product.Publisher} {product.Version}");
        }
    }
}
