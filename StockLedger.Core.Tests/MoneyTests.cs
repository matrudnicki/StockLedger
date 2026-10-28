using Xunit;

namespace StockLedger.Core.Tests;

public class MoneyTests
{
    [Fact]
    public void Constructor_RoundsToTwoDecimalPlaces()
    {
        var money = new Money(10.005m);
        
        Assert.Equal(10.01m, money.Amount);
    }


    [Fact]
    public void Constructor_RoundsNegativeAmountsAwayFromZero()
    {
        var money = new Money(-10.005m);
        
        Assert.Equal(-10.01m, money.Amount);
    }

    [Fact]
    public void Addition_IsExactForDecimals()
    {
        var result = new Money(0.10m) + new Money(0.20m);
        
        Assert.Equal(0.30m, result.Amount);
    }

    [Fact]
    public void Substraction_CanProduceNegativeAmounts()
    {
        var result = new Money(10m) - new Money(15m);
        
        Assert.Equal(new Money(-5m), result);
    }

    [Fact]
    public void Multiplication_ByFactor_RoundsResult()
    {
        var result = new Money(3.33m)* 0.5m;
        
        Assert.Equal(new Money(1.67m), result);
    }
    
    [Fact]
    public void Zero_EqualsDefaultValue()
    {
        Assert.Equal(Money.Zero, default(Money));
    }

    [Fact]
    public void EqualityOperators_CompareAmounts()
    {
        Assert.True(new Money(5m) == new Money(5.00m));
        Assert.True(new Money(5m) != new Money(6m));
    }

    [Fact]
    public void ComparisonOperator_OrderAmounts()
    {
        Assert.True(new Money(5m) < new Money(6m));
        Assert.True(new Money(6m) > new Money(5m));
        Assert.True(new Money(5m) <= new Money(6m));
        Assert.True(new Money(6m) >= new Money(5m));
    }

    [Fact]
    public void EqualAmounts_HaveEqualHashCodes()
    {
        var amounts = new List<Money> {new(1m), new(2m), new(3m)};
        Assert.Equal(new Money(6m), Money.Sum(amounts));
    }

    [Fact]
    public void Sum_OffEmptyCollection_ReturnsZero()
    {
        Assert.Equal(Money.Zero, Money.Sum(new List<Money>()));
    }
    
    
    
}