namespace StockLedger.Core;


/// <summary>Strefa magazynu, np regały chłodnicze, plac</summary>
public class Zone
{
    private readonly List<Location> _locations = new ();

    internal Zone(Warehouse warehouse, string code, string name)
    {
        Warehouse =  warehouse;
        Code = Guard.Code(code, nameof(code));
        Name =  Guard.NotBlank(name, nameof(name));
    }
    
    /// <summary>Magazyn, do którego należy dana strefa</summary>
    public Warehouse Warehouse { get; }
    
    /// <summary>Kod Strefy (wielkimi)</summary>
    public string Code { get; }
    
    /// <summary>Nazwa Strefy</summary>
    public string Name { get; }
    
    /// <summary>Lokalizacje w strefie (rdonly)</summary>
    public IReadOnlyList<Location> Locations => _locations;
    
    
        
    /// <summary>Dodaje lokalizacje do strefy</summary>
    /// <param name="code">kod lokalizacji, unikalny w strefie</param>
    /// <param name="name">Maksymalne obciążenie w kg, większe od zera</param>
    /// <exception cref="DuplicateCodeException">Gdy lokalizacja o takim kodzie istnieje</exception>
    public Location AddLocation(string code, decimal maxLoadKg)
    {
        var normalized = Guard.Code(code, nameof(code));
        if (_locations.Any(location => location.Code == normalized))
        {
            throw new DuplicateCodeException("lokalizacja", normalized);
        }
        var newLocation = new Location(this, normalized, maxLoadKg);
        _locations.Add(newLocation);
        return newLocation;
    }

    ///<inheritdoc />
    public override string ToString() => $"{Warehouse.Code} {Name}";
}