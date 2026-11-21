namespace StockLedger.Core;

/// <summary>
/// Fragment wydania: ile zdjęto z jednej partii i jaki był tego koszt
/// </summary>
public sealed class IssuedPart
{
    public IssuedPart(StockLot lot, Quantity quantity, Money cost)
    {
        Lot = lot;
        Quantity =  quantity;
        Cost = cost;
    }
    public StockLot Lot { get; }
    public Quantity Quantity { get; }
    public Money Cost { get; }
}