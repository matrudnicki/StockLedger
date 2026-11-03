namespace StockLedger.Core;

/// <summary>Próba połączenia ilości w różnych jednostkach.</summary>
public class UnitMismatchException : DomainException
{
    /// <summary>Tworzy wyjątek dla dwóch niezgodnych jednostek.</summary>
    public UnitMismatchException(Unit left, Unit right)
        : base($"Nie można połączyć ilości w różnych jednostkach: {left} i {right}.")
    {
        Left = left;
        Right = right;
    }

    /// <summary>Jednostka lewego argumentu.</summary>
    public Unit Left { get; }

    /// <summary>Jednostka prawego argumentu.</summary>
    public Unit Right { get; }
}