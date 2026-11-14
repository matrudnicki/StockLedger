namespace StockLedger.Core;

/// <summary>
/// Fragment wydania: ile zdjęto z jednej partii i jaki był tego koszt
/// </summary>
public sealed record IssuedPart(StockLot Lot, Quantity Quantity, Money Cost);