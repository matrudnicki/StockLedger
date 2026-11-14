namespace StockLedger.Core;


/// <summary>
/// Metoda wyceny RM (strategia). Decyduje po jakim koszcie jednostkowym towar schodzi z partii (średnia ważona czy FIFO) 
/// </summary>
public interface IValuationMethod
{
    ValuationType Type { get; }
    
    /// <summary>
    /// Przygotowuje partie jednego produktu do wydania. Wywoływanie po sprawdzeniu że towaru wystarczy a przed zdjęciem towaru z partii
    /// </summary>
    void PrepareForIssue(IReadOnlyList<StockLot> lots);
}