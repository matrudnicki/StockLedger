using Xunit;

namespace StockLedger.Core.Tests;

public class QuantityTests
{
    [Fact]
    public void Constructor_StoresValueAndUnit()
    {
        var quantity = new Quantity(2.5m, Unit.Kilogram);

        Assert.Equal(2.5m, quantity.Value);
        Assert.Equal(Unit.Kilogram, quantity.Unit);
    }

    [Fact]
    public void Constructor_ZeroValue_IsAllowed()
    {
        var quantity = new Quantity(0m, Unit.Piece);

        Assert.Equal(0m, quantity.Value);
    }

    [Fact]
    public void Constructor_NegativeValue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Quantity(-1m, Unit.Piece));
    }

    [Fact]
    public void Zero_HasRequestedUnit()
    {
        var zero = Quantity.Zero(Unit.Liter);

        Assert.Equal(0m, zero.Value);
        Assert.Equal(Unit.Liter, zero.Unit);
    }

    [Fact]
    public void Addition_SameUnit_AddsValues()
    {
        var result = new Quantity(1.5m, Unit.Kilogram) + new Quantity(2m, Unit.Kilogram);

        Assert.Equal(new Quantity(3.5m, Unit.Kilogram), result);
    }

    [Fact]
    public void Addition_DifferentUnits_ThrowsUnitMismatchException()
    {
        var exception = Assert.Throws<UnitMismatchException>(
            () => new Quantity(1m, Unit.Kilogram) + new Quantity(1m, Unit.Piece));

        Assert.Equal(Unit.Kilogram, exception.Left);
        Assert.Equal(Unit.Piece, exception.Right);
    }

    [Fact]
    public void Subtraction_SameUnit_SubtractsValues()
    {
        var result = new Quantity(5m, Unit.Piece) - new Quantity(2m, Unit.Piece);

        Assert.Equal(new Quantity(3m, Unit.Piece), result);
    }

    [Fact]
    public void Subtraction_ResultBelowZero_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Quantity(1m, Unit.Piece) - new Quantity(2m, Unit.Piece));
    }

    [Fact]
    public void Equality_RequiresSameValueAndUnit()
    {
        Assert.True(new Quantity(1m, Unit.Kilogram) == new Quantity(1.0m, Unit.Kilogram));
        Assert.True(new Quantity(1m, Unit.Kilogram) != new Quantity(1m, Unit.Gram));
    }

    [Fact]
    public void ComparisonOperators_SameUnit_OrderValues()
    {
        Assert.True(new Quantity(1m, Unit.Piece) < new Quantity(2m, Unit.Piece));
        Assert.True(new Quantity(2m, Unit.Piece) > new Quantity(1m, Unit.Piece));
        Assert.True(new Quantity(2m, Unit.Piece) <= new Quantity(2m, Unit.Piece));
        Assert.True(new Quantity(2m, Unit.Piece) >= new Quantity(2m, Unit.Piece));
    }

    [Fact]
    public void Comparison_DifferentUnits_Throws()
    {
        Assert.Throws<UnitMismatchException>(
            () => new Quantity(1m, Unit.Kilogram) < new Quantity(1m, Unit.Piece));
    }

    [Fact]
    public void EqualQuantities_HaveEqualHashCodes()
    {
        var first = new Quantity(1m, Unit.Liter);
        var second = new Quantity(1.0m, Unit.Liter);

        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void UnitMismatchException_IsDomainException()
    {
        var exception = new UnitMismatchException(Unit.Gram, Unit.Liter);

        Assert.IsAssignableFrom<DomainException>(exception);
        Assert.Contains("Gram", exception.Message);
    }
}