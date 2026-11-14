namespace StockLedger.Core;


/// <summary>
/// Stan magazynu: zbiór partii towarów rozłożonych po różnych lokalizacjach
/// </summary>
public class Stock
{
    private readonly List<StockLot> _lots = new();
    
    
    /// <summary>
    /// Wszystkie partie w magazynie
    /// </summary>
    public IReadOnlyList<StockLot> Lots => _lots;

    /// <summary>
    /// Przymuje towar na lokalizację jako nową partię.
    /// </summary>
    /// <param name="product">Przyjmowany towar</param>
    /// <param name="location">Lokalizacja doeclowa</param>
    /// <param name="quantity">Przyjmowana ilość</param>
    /// <param name="totalValue">Wartość całej przyjętej ilości</param>
    /// <param name="receivedOn">Data przyjęcia</param>
    /// <param name="expiryDate">Data ważności, wymagana gdy towar takową posiada</param>
    /// <returns>Utworzona partia</returns>
    /// <exception cref="LocationCapacityException">Gdy lokalizacja nie pomieści towaru</exception>
    public StockLot Receive(
        Product product,
        Location location,
        Quantity quantity,
        Money totalValue,
        DateOnly receivedOn,
        DateOnly? expiryDate = null
        )
    {
        var lot = new StockLot(product, location, quantity, totalValue, receivedOn, expiryDate);
        var currentLoad = GetLoadKg(location);
        if (currentLoad + lot.LoadKg > location.MaxLoadKg)
        {
        throw new LocationCapacityException(location, currentLoad, lot.LoadKg);
        }
        _lots.Add(lot);
        return lot;
    }

    
    /// <summary>
    /// Łączna ilość towaru we wszystkich partiach
    /// </summary>
    public Quantity GetQuantity(Product product)
    {
        var total = Quantity.Zero(product.Unit);
        foreach (var lot in _lots.Where(candidate => candidate.Product.Sku == product.Sku))
        {
            total += lot.RemainingQuantity;
        }
        return total;
    }

    
    /// <summary>
    /// Łączna wartość we wszystkich partiach
    /// </summary>
    public Money GetValue(Product product) => Money.Sum(_lots
        .Where(candidate => candidate.Product.Sku == product.Sku)
        .Select(candidate => candidate.RemainingValue));

    
    /// <summary>
    /// Obecne obciążenie lokalizacji
    /// </summary>
    public decimal GetLoadKg(Location location) => _lots.Where(candidate => candidate.Location == location).Sum(candidate => candidate.LoadKg);

    
    /// <summary>
    /// Wydaje towar z magazynu. Zwykłe produkty FIFO, produkty z terminem ważności FEFO.
    /// Partie przeterminowane na dzień wydania są pomijane.
    /// </summary>
    /// <exception cref="InsufficientStockException">Brak wystarczającej ilości towaru.</exception>
    public IssueResult Issue(Product product, Quantity quantity, DateOnly issueDate)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (quantity.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Wydawana ilość musi być dodatnia.");
        }

        var candidates = PickingOrder(product, issueDate).ToList();

        // Najpierw sprawdzamy, potem zmieniamy: przy braku towaru stan zostaje nietknięty.
        var available = Quantity.Zero(product.Unit);
        foreach (var lot in candidates)
        {
            available += lot.RemainingQuantity;
        }

        if (available < quantity)
        {
            throw new InsufficientStockException(product, quantity, available);
        }

        var parts = new List<IssuedPart>();
        var left = quantity;

        foreach (var lot in candidates)
        {
            if (left.Value == 0)
            {
                break;
            }

            var take = lot.RemainingQuantity <= left ? lot.RemainingQuantity : left;
            var cost = lot.Take(take);

            parts.Add(new IssuedPart(lot, take, cost));
            left -= take;
        }

        _lots.RemoveAll(l => l.IsEmpty);

        return new IssueResult(product, parts);
    }

    /// <summary>Partie danego produktu w kolejności, w jakiej mają być wydawane.</summary>
    private IEnumerable<StockLot> PickingOrder(Product product, DateOnly issueDate)
    {
        var lots = _lots.Where(l => l.Product.Sku == product.Sku
                                    && !l.IsEmpty
                                    && !l.IsExpiredOn(issueDate));

        return product.TracksExpiry
            ? lots.OrderBy(l => l.ExpiryDate ?? DateOnly.MaxValue).ThenBy(l => l.ReceivedOn)
            : lots.OrderBy(l => l.ReceivedOn);
    }
}