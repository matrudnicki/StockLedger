namespace StockLedger.Core;

/// <summary>
/// Zmienia wartość enuma na obiekt naszej strategii wyceny
/// </summary>
public static class ValuationMethods
{
    public static IValuationMethod For(ValuationType type)
    {
        if (type == ValuationType.Fifo)
        {
            return new FifoValuation();
        }

        if (type == ValuationType.WeightedAverage)
        {
            return new WeightedAverageValuation();
        }

        throw new ArgumentOutOfRangeException(nameof(type), type, "Nieznana metoda wyceny.");
    }
}