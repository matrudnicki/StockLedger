namespace StockLedger.Core;

public class LocationCapacityException : DomainException
{
    /// <summary>
    /// Wyjątek przekroczenia pojemności
    /// </summary>
    public LocationCapacityException(Location location, decimal currentLoadKg, decimal additionalLoadKg) : base(
        $"Location {location.FullCode} cannot hold additional {additionalLoadKg} kg" +
        $"Current load {currentLoadKg} kg, capacity{location.MaxLoadKg}")
    {
        Location = location;
        CurrentLoadKg = currentLoadKg;
        AdditionalLoadKg = additionalLoadKg;
    }
    
    /// <summary>
    /// Lokalizacja gdzie zabrakło miejsca
    /// </summary>
    public Location Location { get; }
    
    /// <summary>
    /// Obecne przeciążenie w kilogramach
    /// </summary>
    public decimal CurrentLoadKg { get; }
    
    /// <summary>
    /// Dodatkowe obciążenie, którego nie da się przyjąć
    /// </summary>
    public decimal AdditionalLoadKg { get; }
}