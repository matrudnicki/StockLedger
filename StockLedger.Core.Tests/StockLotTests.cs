using Xunit;

namespace StockLedger.Core.Tests;

public class StockLotTests
{
    private static readonly DateOnly Today = new(2026, 10, 29);

    private static Quantity Kg(decimal value) => new(value, Unit.Kilogram);

    private static Product CreateProduct(bool tracksExpiry = false) =>
        new("SLOD", "Słód", Unit.Kilogram, 1m, new Money(4m), tracksExpiry);

    private static Location CreateLocation() =>
        new Warehouse("MAG1", "Główny").AddZone("A", "Suche").AddLocation("A-01", 1000m);

    private static StockLot CreateLot(decimal quantity = 10m, decimal value = 100m) =>
        new(CreateProduct(), CreateLocation(), Kg(quantity), new Money(value), Today);

    [Fact]
    public void Constructor_SetsRemainingToReceivedAmounts()
    {
        var lot = CreateLot(10m, 100m);

        Assert.Equal(Kg(10m), lot.RemainingQuantity);
        Assert.Equal(new Money(100m), lot.RemainingValue);
        Assert.Equal(Today, lot.ReceivedOn);
    }

    [Fact]
    public void Constructor_UnitDifferentFromProduct_Throws()
    {
        Assert.Throws<UnitMismatchException>(
            () => new StockLot(CreateProduct(), CreateLocation(), new Quantity(1m, Unit.Piece), new Money(1m), Today));
    }

    [Fact]
    public void Constructor_ZeroQuantity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new StockLot(CreateProduct(), CreateLocation(), Kg(0m), new Money(1m), Today));
    }

    [Fact]
    public void Constructor_NegativeValue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new StockLot(CreateProduct(), CreateLocation(), Kg(1m), new Money(-1m), Today));
    }

    [Fact]
    public void Constructor_ProductWithExpiryButNoDate_Throws()
    {
        Assert.Throws<DomainException>(
            () => new StockLot(CreateProduct(tracksExpiry: true), CreateLocation(), Kg(1m), new Money(1m), Today));
    }

    [Fact]
    public void Constructor_ExpiryBeforeReceipt_Throws()
    {
        Assert.Throws<ArgumentException>(
            () => new StockLot(CreateProduct(tracksExpiry: true), CreateLocation(), Kg(1m), new Money(1m), Today, Today.AddDays(-1)));
    }

    [Fact]
    public void Number_HasPrefixAndIsUnique()
    {
        var first = CreateLot();
        var second = CreateLot();

        Assert.StartsWith("L-", first.Number);
        Assert.NotEqual(first.Number, second.Number);
    }

    [Fact]
    public void Take_PartOfLot_ReducesQuantityAndValue()
    {
        var lot = CreateLot(10m, 100m);

        var cost = lot.Take(Kg(4m));

        Assert.Equal(new Money(40m), cost);
        Assert.Equal(Kg(6m), lot.RemainingQuantity);
        Assert.Equal(new Money(60m), lot.RemainingValue);
    }

    [Fact]
    public void Take_WholeLot_ReturnsAllRemainingValue()
    {
        var lot = CreateLot(10m, 100m);

        var cost = lot.Take(Kg(10m));

        Assert.Equal(new Money(100m), cost);
        Assert.True(lot.IsEmpty);
        Assert.Equal(Money.Zero, lot.RemainingValue);
    }

    [Fact]
    public void Take_InPieces_DoesNotLoseOrInventGrosze()
    {
        var lot = CreateLot(3m, 10m);

        var total = Money.Sum(new[] { lot.Take(Kg(1m)), lot.Take(Kg(1m)), lot.Take(Kg(1m)) });

        Assert.Equal(new Money(10m), total);
        Assert.Equal(Money.Zero, lot.RemainingValue);
    }

    [Fact]
    public void Take_MoreThanAvailable_ThrowsAndLeavesLotUnchanged()
    {
        var lot = CreateLot(10m, 100m);

        var exception = Assert.Throws<InsufficientStockException>(() => lot.Take(Kg(11m)));

        Assert.IsAssignableFrom<DomainException>(exception);
        Assert.Equal(Kg(11m), exception.Requested);
        Assert.Equal(Kg(10m), exception.Available);
        Assert.Equal(Kg(10m), lot.RemainingQuantity);
        Assert.Equal(new Money(100m), lot.RemainingValue);
    }

    [Fact]
    public void Take_DifferentUnit_Throws()
    {
        var lot = CreateLot();

        Assert.Throws<UnitMismatchException>(() => lot.Take(new Quantity(1m, Unit.Piece)));
    }

    [Fact]
    public void Take_ZeroQuantity_Throws()
    {
        var lot = CreateLot();

        Assert.Throws<ArgumentOutOfRangeException>(() => lot.Take(Kg(0m)));
    }

    [Fact]
    public void IsExpiredOn_ComparesWithExpiryDate()
    {
        var lot = new StockLot(CreateProduct(tracksExpiry: true), CreateLocation(), Kg(10m), new Money(100m), Today, Today.AddDays(2));

        Assert.False(lot.IsExpiredOn(Today));
        Assert.False(lot.IsExpiredOn(Today.AddDays(2)));
        Assert.True(lot.IsExpiredOn(Today.AddDays(3)));
    }

    [Fact]
    public void LoadKg_IsRemainingQuantityTimesUnitWeight()
    {
        var product = new Product("PIWO", "Piwo", Unit.Piece, 0.5m, new Money(6m));
        var lot = new StockLot(product, CreateLocation(), new Quantity(10m, Unit.Piece), new Money(60m), Today);

        Assert.Equal(5m, lot.LoadKg);
    }
}