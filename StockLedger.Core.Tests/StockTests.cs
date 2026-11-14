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
    private static Product CreatePerishable() =>
    new("MLEKO", "Mleko", Unit.Kilogram, 1m, new Money(4m), tracksExpiry: true);

    [Fact]
    public void Issue_TakesFromOldestLotFirst()
    {
        var stock = new Stock();
        var product = CreateProduct();
        var location = CreateLocation();
        var newer = stock.Receive(product, location, Kg(10m), new Money(60m), Today.AddDays(-1));
        var older = stock.Receive(product, location, Kg(10m), new Money(40m), Today.AddDays(-3));

        var result = stock.Issue(product, Kg(5m), Today);

        var part = Assert.Single(result.Parts);
        Assert.Same(older, part.Lot);
        Assert.Equal(new Money(20m), result.TotalCost);
        Assert.Equal(Kg(5m), older.RemainingQuantity);
        Assert.Equal(Kg(10m), newer.RemainingQuantity);
    }

    [Fact]
    public void Issue_SpansMultipleLots()
    {
        var stock = new Stock();
        var product = CreateProduct();
        var location = CreateLocation();
        stock.Receive(product, location, Kg(10m), new Money(40m), Today.AddDays(-2));
        var second = stock.Receive(product, location, Kg(10m), new Money(60m), Today.AddDays(-1));

        var result = stock.Issue(product, Kg(15m), Today);

        Assert.Equal(2, result.Parts.Count);
        Assert.Equal(new Money(40m), result.Parts[0].Cost);
        Assert.Equal(new Money(30m), result.Parts[1].Cost);
        Assert.Equal(new Money(70m), result.TotalCost);
        Assert.Equal(Kg(15m), result.TotalQuantity);
        Assert.Equal(new Money(30m), second.RemainingValue);
    }

    [Fact]
    public void Issue_RemovesEmptyLots()
    {
        var stock = new Stock();
        var product = CreateProduct();
        stock.Receive(product, CreateLocation(), Kg(10m), new Money(40m), Today);

        stock.Issue(product, Kg(10m), Today);

        Assert.Empty(stock.Lots);
    }

    [Fact]
    public void Issue_MoreThanAvailable_ThrowsAndChangesNothing()
    {
        var stock = new Stock();
        var product = CreateProduct();
        var lot = stock.Receive(product, CreateLocation(), Kg(10m), new Money(40m), Today);

        Assert.Throws<InsufficientStockException>(() => stock.Issue(product, Kg(15m), Today));

        Assert.Equal(Kg(10m), lot.RemainingQuantity);
        Assert.Equal(Kg(10m), stock.GetQuantity(product));
    }

    [Fact]
    public void Issue_NonPositiveQuantity_Throws()
    {
        var stock = new Stock();
        var product = CreateProduct();
        stock.Receive(product, CreateLocation(), Kg(10m), new Money(40m), Today);

        Assert.Throws<ArgumentOutOfRangeException>(() => stock.Issue(product, Kg(0m), Today));
    }

    [Fact]
    public void Issue_ProductWithExpiry_TakesEarliestExpiryFirst()
    {
        var stock = new Stock();
        var milk = CreatePerishable();
        var location = CreateLocation();
        // Starsze przyjęcie, ale dłuższy termin: FEFO ma wybrać drugą partię.
        stock.Receive(milk, location, Kg(10m), new Money(40m), Today.AddDays(-5), Today.AddDays(20));
        var shorter = stock.Receive(milk, location, Kg(10m), new Money(40m), Today.AddDays(-1), Today.AddDays(3));

        var result = stock.Issue(milk, Kg(5m), Today);

        var part = Assert.Single(result.Parts);
        Assert.Same(shorter, part.Lot);
    }

    [Fact]
    public void Issue_SkipsExpiredLots()
    {
        var stock = new Stock();
        var milk = CreatePerishable();
        var location = CreateLocation();
        stock.Receive(milk, location, Kg(10m), new Money(40m), Today.AddDays(-10), Today.AddDays(-1));
        var fresh = stock.Receive(milk, location, Kg(10m), new Money(40m), Today.AddDays(-1), Today.AddDays(10));

        var result = stock.Issue(milk, Kg(5m), Today);

        var part = Assert.Single(result.Parts);
        Assert.Same(fresh, part.Lot);
    }

    [Fact]
    public void Issue_ExpiredStockDoesNotCountAsAvailable()
    {
        var stock = new Stock();
        var milk = CreatePerishable();
        stock.Receive(milk, CreateLocation(), Kg(10m), new Money(40m), Today.AddDays(-10), Today.AddDays(-1));

        Assert.Throws<InsufficientStockException>(() => stock.Issue(milk, Kg(1m), Today));
    }
}
