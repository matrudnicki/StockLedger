namespace StockLedger.Core;

using System.Globalization;

///<summary>
///Kwota w złotych zaokrąglona do groszy. Typ jest nieznany.
/// </summary>


public readonly struct Money : IEquatable<Money>, IComparable<Money>
{
    private readonly decimal _amount;

    ///<summary>Tworzy kwotę i zaokrągla ją do groszy.</summary>
    public Money(decimal amount)
    {
        _amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }
    /// <summary>Wartość liczbowa kwoty</summary>
    public decimal Amount => _amount;
    
    /// <summary>Kwota zero</summary>
    public static Money Zero => new(0m);

    /// <summary>Suma dwóch kwot</summary>
    public static Money operator +(Money left, Money right) => new(left._amount + right._amount);
    
    /// <summary>Różnica dwóch kwot</summary>
    public static Money operator -(Money left, Money right) => new(left._amount -  right._amount);
    
    /// <summary>Kwota pomnożona przez współczynnik, np. ilość lub udział</summary>
    public static Money operator *(Money money, decimal factor) => new(money._amount * factor);


    ///<inheritdoc />
    public bool Equals(Money other) => _amount == other._amount;
    
    ///<inheritdoc />
    public override bool Equals(object? obj) => obj is Money other && Equals(other);
    
    ///<inheritdoc />
    public override int GetHashCode() => _amount.GetHashCode();
    
    ///<inheritdoc />
    public int CompareTo(Money other) => _amount.CompareTo(other.Amount);
    
    ///<summary>Czy kwoty są równe</summary>
    public static bool operator ==(Money left, Money right) => left.Equals(right);
    
    ///<summary>Czy kwoty są różne</summary>
    public static bool operator !=(Money left, Money right) => !left.Equals(right);
    
    ///<summary>Czy lewa jest mniejsza</summary>
    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;
    
    ///<summary>Czy lewa jest większa</summary>
    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;
    
    ///<summary>Czy lewa jest mniejsza lub równa</summary>
    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;
    
    ///<summary>Czy lewa jest większa lub równa</summary>
    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;

    ///<summary>Sumuje kolekcję kwot</summary>
    public static Money Sum(IEnumerable<Money> amounts)
    {
        var total = Zero;
        foreach (var amount in amounts)
        {
            total += amount;
        }
        return total;
    }
    
    ///<inheritdoc />
    public override string ToString() => _amount.ToString(CultureInfo.InvariantCulture);
    
    
}