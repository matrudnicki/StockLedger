using Xunit;

namespace StockLedger.Core.Tests;

public class ProductTests
{
    private static Product CreateProduct() => new("slod-pils", "Słód Pilzneński", Unit.Kilogram, 1m, new Money(4.50m));

    [Fact]
    public void Constructor_NormalizesSku()
    {
        Assert.Equal("SLOD-PILS", CreateProduct().Sku);
    }

    [Fact]
    public void Constructor_UsesFifoAndNoExpirByDefault()
    {
        var product = CreateProduct();
        Assert.Equal(ValuationType.Fifo, product.Valuation);
        Assert.False(product.TracksExpiry);
    }

    [Fact]
    public void Contructor_BlankSku_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Product("", "Słód", Unit.Kilogram, 1m, new Money(1m)));
    }

    [Fact]
    public void Constructor_NonPositiveWeight_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Product("slod", "Słód", Unit.Kilogram, 0m, new Money(1m)));
    }

    [Fact]
    public void Name_SetBlanks_Throws()
    {
        var product = CreateProduct();
        Assert.Throws<ArgumentException>(() => product.Name = "");
    }

    [Fact]
    public void Name_Set_TrimsAndStoresValue()
    {
        var product = CreateProduct();
        product.Name = "Słód z Monachium";
        Assert.Equal("Słód z Monachium", product.Name);
    }
    
    [Fact]
    public void ListPrice_SetNegative_Throws()
    {
        var product = CreateProduct();
        Assert.Throws<ArgumentOutOfRangeException>(() => product.ListPrice = new Money(-1m));
    }

    [Fact]
    public void ListPrice_Set_UpdatesPrice()
    {
        var product = CreateProduct();
        product.ListPrice = new Money(5m);
        Assert.Equal(new Money(5m), product.ListPrice);
    }
    
}