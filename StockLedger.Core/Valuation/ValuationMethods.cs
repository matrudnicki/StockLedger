namespace StockLedger.Core;

/// <summary>
/// Zmienia wartość enuma na obiekt naszej strategii wyceny
/// </summary>
public static class ValuationMethods
{
    public static IValuationMethod For(ValuationType type) => type switch
    {
        ValuationType.Fifo => new FifoValuation(),
        ValuationType.WeightedAverage => new WeightedAverageValuation(),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Nieznana metoda wyceny")
    };
}