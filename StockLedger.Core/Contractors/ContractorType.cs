namespace StockLedger.Core;

/// <summary>Rola kontrahenta w obrocie towarem.</summary>
public enum ContractorType
{
    /// <summary>Dostawca: firma, od której przyjmujemy towar (PZ)</summary>
    Supplier = 1,
    
    /// <summary>Odbiorca: firma, której wydajemy towar (WZ)</summary>
    Customer = 2
}