namespace StockLedger.Core;

/// <summary>Partia towaru w konkretnej lokalizacji, z ilością, wartością i opcjonalną datą ważności.</summary>
public class StockLot
{
    private static int _lastNumber;

    internal StockLot(
        Product product,
        Location location,
        Quantity quantity,
        Money totalValue,
        DateOnly receivedOn,
        DateOnly? expiryDate = null)
    {
        if (quantity.Unit != product.Unit)
        {
            throw new UnitMismatchException(product.Unit, quantity.Unit);
        }

        Guard.Positive(quantity.Value, nameof(quantity));

        if (totalValue < Money.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(totalValue), totalValue, "Number of the party cannot be negative.");
        }

        if (product.TracksExpiry && expiryDate is null)
        {
            throw new DomainException($"Product {product.Sku} requries expiry date.");
        }

        if (expiryDate is not null && expiryDate < receivedOn)
        {
            throw new ArgumentException("Expiry date cannot be before the received date.", nameof(expiryDate));
        }

        Number = $"L-{Interlocked.Increment(ref _lastNumber):D6}";
        Product = product;
        Location = location;
        RemainingQuantity = quantity;
        RemainingValue = totalValue;
        ReceivedOn = receivedOn;
        ExpiryDate = expiryDate;
    }

    /// <summary>Numer partii, np. L-000001.</summary>
    public string Number { get; }

    /// <summary>Towar w partii.</summary>
    public Product Product { get; }

    /// <summary>Lokalizacja, w której leży partia.</summary>
    public Location Location { get; }

    /// <summary>Ilość pozostała w partii.</summary>
    public Quantity RemainingQuantity { get; private set; }

    /// <summary>Wartość pozostałej ilości.</summary>
    public Money RemainingValue { get; private set; }

    /// <summary>Data przyjęcia partii.</summary>
    public DateOnly ReceivedOn { get; }

    /// <summary>Data ważności lub null, gdy towar jej nie ma.</summary>
    public DateOnly? ExpiryDate { get; }

    /// <summary>Czy partia jest pusta.</summary>
    public bool IsEmpty => RemainingQuantity.Value == 0;

    /// <summary>Obciążenie lokalizacji przez tę partię w kilogramach.</summary>
    public decimal LoadKg => RemainingQuantity.Value * Product.UnitWeightKg;

    /// <summary>Czy partia jest przeterminowana w podanym dniu.</summary>
    /// <param name="date">Dzień, na który sprawdzamy.</param>
    public bool IsExpiredOn(DateOnly date) => ExpiryDate is not null && ExpiryDate < date;

    /// <summary>Pobiera ilość z partii i zwraca jej koszt.</summary>
    /// <param name="quantity">Pobierana ilość, większa od zera.</param>
    /// <returns>Wartość pobranej ilości.</returns>
    /// <exception cref="UnitMismatchException">Gdy jednostka jest inna niż w partii.</exception>
    /// <exception cref="InsufficientStockException">Gdy w partii jest za mało towaru.</exception>
    public Money Take(Quantity quantity)
    {
        Guard.Positive(quantity.Value, nameof(quantity));

        if (quantity > RemainingQuantity)
        {
            throw new InsufficientStockException(Product, quantity, RemainingQuantity);
        }

        var cost = quantity == RemainingQuantity
            ? RemainingValue
            : new Money(RemainingValue.Amount * quantity.Value / RemainingQuantity.Value);

        RemainingQuantity -= quantity;
        RemainingValue -= cost;
        return cost;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Number} {Product.Sku} {RemainingQuantity} @ {Location.FullCode}";
}