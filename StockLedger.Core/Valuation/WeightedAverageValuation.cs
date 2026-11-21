namespace StockLedger.Core;

/// <summary>
/// Metoda wyceny: Średnia ważona - przed wydaniem wszystkie partie dostają ten sam koszt jednostkowy, łączna wartość dzielona przez łączną ilość.
/// </summary>

internal sealed class WeightedAverageValuation : IValuationMethod
{
    public ValuationType Type => ValuationType.WeightedAverage;

    public void PrepareForIssue(IReadOnlyList<StockLot> lots)
    {
        if (lots.Count <= 1)
        {
            return;
        }

        var totalValue = Money.Sum(lots.Select(l => l.RemainingValue));
        var totalQuantity = lots.Sum(l => l.RemainingQuantity.Value);
        var assigned = Money.Zero;
        for (var i = 0; i < lots.Count - 1; i++)
        {
            var share = new Money(totalValue.Amount * lots[i].RemainingQuantity.Value / totalQuantity);
            lots[i].Revalue(share);
            assigned += share;
        }
        var lastlot = lots[lots.Count - 1];
        lastlot.Revalue(totalValue - assigned);
    }
}