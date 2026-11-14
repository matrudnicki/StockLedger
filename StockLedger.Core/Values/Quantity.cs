using System.Globalization;

namespace StockLedger.Core;

/// <summary>Nieujemna ilość towaru wraz z jednostką miary. Typ jest niezmienny.</summary>
public readonly struct Quantity : IEquatable<Quantity>, IComparable<Quantity>
{
    /// <summary>Tworzy ilość.</summary>
    public Quantity(decimal value, Unit unit)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Ilość nie może być ujemna.");
        }

        Value = value;
        Unit = unit;
    }

    /// <summary>Wartość liczbowa ilości.</summary>
    public decimal Value { get; }

    /// <summary>Jednostka miary.</summary>
    public Unit Unit { get; }

    /// <summary>Ilość zero w podanej jednostce.</summary>
    public static Quantity Zero(Unit unit) => new(0m, unit);

    /// <summary>Suma ilości w tej samej jednostce.</summary>
    public static Quantity operator +(Quantity left, Quantity right)
    {
        EnsureSameUnit(left, right);
        return new Quantity(left.Value + right.Value, left.Unit);
    }

    /// <summary>Różnica ilości w tej samej jednostce.</summary>
    public static Quantity operator -(Quantity left, Quantity right)
    {
        EnsureSameUnit(left, right);
        return new Quantity(left.Value - right.Value, left.Unit);
    }

    /// <summary>Czy ilości są równe (wartość i jednostka).</summary>
    public static bool operator ==(Quantity left, Quantity right) => left.Equals(right);

    /// <summary>Czy ilości są różne.</summary>
    public static bool operator !=(Quantity left, Quantity right) => !left.Equals(right);

    /// <summary>Czy lewa ilość jest mniejsza.</summary>
    public static bool operator <(Quantity left, Quantity right) => left.CompareTo(right) < 0;

    /// <summary>Czy lewa ilość jest większa.</summary>
    public static bool operator >(Quantity left, Quantity right) => left.CompareTo(right) > 0;

    /// <summary>Czy lewa ilość jest mniejsza lub równa.</summary>
    public static bool operator <=(Quantity left, Quantity right) => left.CompareTo(right) <= 0;

    /// <summary>Czy lewa ilość jest większa lub równa.</summary>
    public static bool operator >=(Quantity left, Quantity right) => left.CompareTo(right) >= 0;

    /// <inheritdoc />
    public bool Equals(Quantity other) => Value == other.Value && Unit == other.Unit;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Quantity other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Value, Unit);

    /// <inheritdoc />
    public int CompareTo(Quantity other)
    {
        EnsureSameUnit(this, other);
        return Value.CompareTo(other.Value);
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"{Value.ToString("0.###", CultureInfo.GetCultureInfo("pl-PL"))} {Unit}";

    private static void EnsureSameUnit(Quantity left, Quantity right)
    {
        if (left.Unit != right.Unit)
        {
            throw new UnitMismatchException(left.Unit, right.Unit);
        }
    }
}
