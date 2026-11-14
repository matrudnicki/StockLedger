namespace StockLedger.Core;

/// <summary>
/// Wynik wydania towaru: z których partii zdjęto towar i ile to kosztowało
/// </summary>
public sealed class IssueResult
{
    public IssueResult(Product product, IReadOnlyList<IssuedPart> parts)
    {
        ArgumentNullException.ThrowIfNull(parts);
        ArgumentNullException.ThrowIfNull(product);
        Product = product;
        Parts = parts;
    }

    public Product Product { get; }
    public IReadOnlyList<IssuedPart> Parts { get; }
    
    /// <summary>
    /// Łączny koszt wydania (suma kosztów ze wszystkich partii)
    /// </summary>
    public Money TotalCost => Money.Sum(Parts.Select(p => p.Cost));

    /// <summary>
    /// Łączna wydana ilość
    /// </summary>
    public Quantity TotalQuantity
    {
        get
        {
            var total = Quantity.Zero(Product.Unit);
            foreach (var part in Parts)
            {
                total += part.Quantity;
            }
            return total;
        }
    }
        
        
}