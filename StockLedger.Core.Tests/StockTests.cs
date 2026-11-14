using Xunit;

namespace StockLedger.Core.Tests;

public class StockTests
{
    private static readonly DateOnly Today = new(2026, 10, 29);

    private static Quantity Kg(decimal value) => new(value, Unit.Kilogram);

    private static Product CreateProduct(string sku = "SLOD", decimal unitWeightKg = 1m) =>
        new(sku, "Towar " + sku, Unit.Kilogram, unitWeightKg, new Money(4m));

    private static Location CreateLocation(decimal maxLoadKg = 1000m) =>
        new Warehouse("MAG1", "Główny").AddZone("A", "Suche").AddLocation("A-01", maxLoadKg);

    [Fact]
    public void Receive_AddsLotToStock()
    {
        var stock = new Stock();

        var lot = stock.Receive(CreateProduct(), CreateLocation(), Kg(10m), new Money(40m), Today);

        Assert.Same(lot, Assert.Single(stock.Lots));
        Assert.Equal(Kg(10m), lot.RemainingQuantity);
        Assert.Equal(new Money(40m), lot.RemainingValue);
    }

    [Fact]
    public void Receive_UpToExactCapacity_IsAllowed()
    {
        var stock = new Stock();
        var location = CreateLocation(maxLoadKg: 100m);

        stock.Receive(CreateProduct(), location, Kg(60m), new Money(1m), Today);
        stock.Receive(CreateProduct(), location, Kg(40m), new Money(1m), Today);

        Assert.Equal(100m, stock.GetLoadKg(location));
    }

    [Fact]
    public void Receive_ExceedingCapacity_ThrowsAndAddsNothing()
    {
        var stock = new Stock();
        var location = CreateLocation(maxLoadKg: 100m);
        stock.Receive(CreateProduct(), location, Kg(60m), new Money(1m), Today);

        var exception = Assert.Throws<LocationCapacityException>(
            () => stock.Receive(CreateProduct(), location, Kg(50m), new Money(1m), Today));

        Assert.Same(location, exception.Location);
        Assert.Equal(60m, exception.CurrentLoadKg);
        Assert.Equal(50m, exception.AdditionalLoadKg);
        Assert.Single(stock.Lots);
    }

    [Fact]
    public void Receive_UsesUnitWeightOfProduct()
    {
        var stock = new Stock();
        var location = CreateLocation(maxLoadKg: 100m);
        var heavy = CreateProduct(unitWeightKg: 2m);

        stock.Receive(heavy, location, Kg(50m), new Money(1m), Today);

        Assert.Throws<LocationCapacityException>(
            () => stock.Receive(heavy, location, Kg(1m), new Money(1m), Today));
    }

    [Fact]
    public void Receive_InvalidLot_DoesNotChangeStock()
    {
        var stock = new Stock();

        Assert.Throws<UnitMismatchException>(
            () => stock.Receive(CreateProduct(), CreateLocation(), new Quantity(1m, Unit.Piece), new Money(1m), Today));

        Assert.Empty(stock.Lots);
    }

    [Fact]
    public void GetQuantity_SumsLotsOfTheProductOnly()
    {
        var stock = new Stock();
        var location = CreateLocation();
        var malt = CreateProduct("SLOD");
        var hops = CreateProduct("CHMIEL");
        stock.Receive(malt, location, Kg(5m), new Money(20m), Today);
        stock.Receive(malt, location, Kg(7m), new Money(30m), Today);
        stock.Receive(hops, location, Kg(3m), new Money(9m), Today);

        Assert.Equal(Kg(12m), stock.GetQuantity(malt));
    }

    [Fact]
    public void GetQuantity_ProductWithoutLots_IsZeroInProductUnit()
    {
        var stock = new Stock();

        Assert.Equal(Kg(0m), stock.GetQuantity(CreateProduct()));
    }

    [Fact]
    public void GetValue_SumsLotValuesOfTheProductOnly()
    {
        var stock = new Stock();
        var location = CreateLocation();
        var malt = CreateProduct("SLOD");
        var hops = CreateProduct("CHMIEL");
        stock.Receive(malt, location, Kg(5m), new Money(20m), Today);
        stock.Receive(malt, location, Kg(7m), new Money(30.50m), Today);
        stock.Receive(hops, location, Kg(3m), new Money(9m), Today);

        Assert.Equal(new Money(50.50m), stock.GetValue(malt));
    }

    [Fact]
    public void GetLoadKg_CountsOnlyGivenLocation()
    {
        var stock = new Stock();
        var zone = new Warehouse("MAG1", "Główny").AddZone("A", "Suche");
        var first = zone.AddLocation("A-01", 100m);
        var second = zone.AddLocation("A-02", 100m);
        stock.Receive(CreateProduct(), first, Kg(10m), new Money(1m), Today);
        stock.Receive(CreateProduct(), second, Kg(20m), new Money(1m), Today);

        Assert.Equal(10m, stock.GetLoadKg(first));
        Assert.Equal(20m, stock.GetLoadKg(second));
    }

    [Fact]
    public void LocationCapacityException_IsDomainException()
    {
        var exception = new LocationCapacityException(CreateLocation(), 1m, 2m);

        Assert.IsAssignableFrom<DomainException>(exception);
    }
}
