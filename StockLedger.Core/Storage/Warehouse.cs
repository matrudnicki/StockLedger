namespace StockLedger.Core;




/// <summary>
/// Magazyn dzielony na strefy
/// </summary>
public class Warehouse
{
    private readonly List<Zone> _zones = new();

    /// <summary>Tworzy magazyn</summary>
    /// <param name="code">Unikalny kod magazynu (np. MG0001)</param>
    /// <param name="name">Nazwa magazynu</param>
    public Warehouse(string code, string name)
    {
        Code = Guard.Code(code, nameof(code));
        Name = Guard.NotBlank(name, nameof(name));
    }
    
    /// <summary>Kod magazynu (wielkimi literami - podobny case do nazwy produktu) </summary>
    public string Code { get; }
    
    /// <summary>Nazwa magazynu </summary>
    public string Name { get; }
    
    /// <summary>Strefy magazynu</summary>
    public IReadOnlyList<Zone> Zones => _zones;

    
    /// <summary>Wszystkie lokalizacje ze wszysktkich stref </summary>
    public IEnumerable<Location> Locations => _zones.SelectMany(zone => zone.Locations);

    
    /// <summary> Dodaje strefę magazynu</summary>
    /// <param name="code">kod strefy, unikalny w magazynie</param>
    /// <param name="name">Nazwa strefy</param>
    /// <exception cref="DuplicateCodeException">Gdy strefa o danym kodzie już istnieje</exception>
    public Zone AddZone(string code, string name)
    {
        var normalized = Guard.Code(code, nameof(code));
        if (_zones.Any(zone => zone.Code == normalized))
        {
            throw new DuplicateCodeException("Strefa", normalized);
        }
        
        var newZone = new Zone(this, normalized, name);
        _zones.Add(newZone);
        return newZone;
    }
    
    ///<inheritdoc />
    public override string ToString() => $"{Code}  {Name}";
}