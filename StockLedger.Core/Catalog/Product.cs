namespace StockLedger.Core;


/// <summary>Towar w kartotece magazynowej, identyfikowany przez SKU</summary>
public class Product
{
    private string _name =  string.Empty;
    private Money _listprice;

    public Product(string sku, string name, Unit unit, decimal unitWeightKg, Money listPrice, bool tracksExpiry = false,
        ValuationType valuation = ValuationType.Fifo)
    {
        Sku = Guard.NotBlank(sku, nameof(sku)).ToUpperInvariant();
        Name = name;
        Unit = unit;
        UnitWeightKg = Guard.Positive(unitWeightKg, nameof(unitWeightKg));
        ListPrice = listPrice;
        TracksExpiry = tracksExpiry;
        Valuation = valuation;
    }

    /// <summary>Unikalny kod towaru (wielkimi literami, aby sld-001 i SLD-001 było traktowane jak jeden towar) </summary>
    public string Sku { get; }

    /// <summary>
    /// Nazwa towaru
    /// </summary>
    /// <exception cref="ArgumentException">Gdy nazwa jest pusta</exception>
    public string Name
    {
        get => _name;
        set => _name = Guard.NotBlank(value, nameof(value));
    }
    
    /// <summary>Jednostka miary produktu</summary>
    public Unit Unit { get; }
    
    
    /// <summary>Waga jednej jednostki produktu w kg</summary>
    public decimal UnitWeightKg { get; }

    
    /// <summary>Domyślna cena sprzedaży</summary>
    /// <exception cref="ArgumentOutOfRangeException">Gdy cena jest ujemna</exception>
    public Money ListPrice
    {
        get => _listprice;
        set
        {
            if (value < Money.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Price cannot be less than zero.");
            }
            
            _listprice = value;
        }
    }
    
    /// <summary>Czy towar ma datę ważności</summary>
    public bool TracksExpiry { get; }
    
    
    /// <summary>Metoda wyceny wartości towaru </summary>
    public ValuationType Valuation { get; }
    
    /// <inheritdoc/>

    public override string ToString() => $"{Sku} {Name}";
}