namespace StockLedger.Core;


/// <summary>
/// Każda partia zachowuje własny koszt
/// </summary>
public sealed class FifoValuation : IValuationMethod
{
    public ValuationType Type => ValuationType.Fifo;

    public void PrepareForIssue(IReadOnlyList<StockLot> lots)
    {
        // koszt schodzi z partii dokładnie tak jak został przyjęty.
    }
}
