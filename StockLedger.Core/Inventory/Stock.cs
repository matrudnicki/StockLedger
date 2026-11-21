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

    
      /// <summary>Łączna ilość towaru we wszystkich partiach</summary>
    public Quantity GetQuantity(Product product)
    {
        var total = Quantity.Zero(product.Unit);
        foreach (var lot in LotsOf(product))
        {
            total += lot.RemainingQuantity;
        }
        return total;
    }

    /// <summary>Łączna wartość towaru we wszystkich partiach</summary>
    public Money GetValue(Product product)
    {
        var total = Money.Zero;
        foreach (var lot in LotsOf(product))
        {
            total += lot.RemainingValue;
        }
        return total;
    }

    /// <summary>Obecne obciążenie lokalizacji w kg</summary>
    public decimal GetLoadKg(Location location)
    {
        return _lots.Where(lot => lot.Location == location).Sum(lot => lot.LoadKg);
    }

    /// <summary>
    /// Wydaje towar z magazynu. Zwykłe produkty FIFO, produkty z terminem ważności FEFO.
    /// Partie przeterminowane na dzień wydania są pomijane.
    /// </summary>
    /// <exception cref="InsufficientStockException">Brak wystarczającej ilości towaru</exception>
    public IssueResult Issue(Product product, Quantity quantity, DateOnly issueDate)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (quantity.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Wydawana ilość musi być dodatnia.");
        }

        var candidates = GetPickingOrder(product, issueDate);

        // Najpierw sprawdzamy, potem zmieniamy: przy braku towaru stan zostaje nietknięty.
        CheckAvailable(product, quantity, candidates);

        // Metoda wyceny produktu ustawia koszty partii przed wydaniem.
        ValuationMethods.For(product.Valuation).PrepareForIssue(LotsOf(product));

        var parts = TakeFromLots(quantity, candidates);
        _lots.RemoveAll(lot => lot.IsEmpty);

        return new IssueResult(product, parts);
    }

    /// <summary>Niepuste partie danego produktu</summary>
    private List<StockLot> LotsOf(Product product)
    {
        return _lots.Where(lot => lot.Product.Sku == product.Sku && !lot.IsEmpty).ToList();
    }

    /// <summary>
    /// Partie do wydania w kolejności: FEFO dla towarów z terminem ważności, FIFO dla pozostałych.
    /// Przeterminowane są pomijane.
    /// </summary>
    private List<StockLot> GetPickingOrder(Product product, DateOnly issueDate)
    {
        var usable = LotsOf(product).Where(lot => !lot.IsExpiredOn(issueDate));

        if (product.TracksExpiry)
        {
            // Partia bez terminu (nie powinna się zdarzyć) trafia na koniec kolejki.
            return usable.OrderBy(lot => lot.ExpiryDate ?? DateOnly.MaxValue)
                .ThenBy(lot => lot.ReceivedOn)
                .ToList();
        }

        return usable.OrderBy(lot => lot.ReceivedOn).ToList();
    }

    /// <summary>Rzuca wyjątek, jeśli w podanych partiach jest mniej towaru niż żądano</summary>
    private static void CheckAvailable(Product product, Quantity requested, List<StockLot> candidates)
    {
        var available = Quantity.Zero(product.Unit);
        foreach (var lot in candidates)
        {
            available += lot.RemainingQuantity;
        }

        if (available < requested)
        {
            throw new InsufficientStockException(product, requested, available);
        }
    }

    /// <summary>Zdejmuje żądaną ilość z kolejnych partii i zwraca listę fragmentów wydania</summary>
    private static List<IssuedPart> TakeFromLots(Quantity requested, List<StockLot> candidates)
    {
        var parts = new List<IssuedPart>();
        var left = requested;

        foreach (var lot in candidates)
        {
            if (left.Value == 0)
            {
                break;
            }
            var take = left;
            if (lot.RemainingQuantity < left)
            {
                take = lot.RemainingQuantity;
            }

            var cost = lot.Take(take);
            parts.Add(new IssuedPart(lot, take, cost));
            left -= take;
        }

        return parts;
    }
}