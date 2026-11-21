namespace StockLedger.Core;

public class LocationCapacityException : DomainException
{
    /// <summary>
    /// Wyjątek przekroczenia pojemności
    /// </summary>
    public LocationCapacityException(Location location, decimal currentLoadKg, decimal additionalLoadKg) : base(
        $"Lokalizacja {location.FullCode} nie pomieści dodatkowych {additionalLoadKg} kg. " +
        $"Obecne obciążenie: {currentLoadKg} kg, udźwig: {location.MaxLoadKg} kg.")
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