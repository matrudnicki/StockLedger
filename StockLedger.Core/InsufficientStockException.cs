namespace StockLedger.Core;


/// <summary>
/// Próba pobrania większego towaru niż dostępny
/// </summary>
public class InsufficientStockException : DomainException
{
    
    /// <summary>
    /// Wyjątek braku stanu
    /// </summary>
    /// <param name="product">Towar, którego brakuje</param>
    /// <param name="quantity">Żądana ilość</param>
    /// <param name="available">Dostępna ilość</param>
    public InsufficientStockException(Product product, Quantity quantity, Quantity available) : base(
        $"Insufficient stock of the {product.Sku}: Requested {quantity}, available {available}.")
    {
        Product = product;
        Requested = quantity;
        Available = available;
    }
    
    public Product Product { get; }
    public Quantity Requested { get; }
    public Quantity Available { get; }
    
}