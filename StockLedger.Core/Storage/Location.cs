namespace StockLedger.Core;


/// <summary>Miejsce składowania w strefie, np. półka</summary>
public class Location
{
    internal Location(Zone zone, string code, decimal maxLoadKg)
    {
        Zone = zone;
        Code =  Guard.Code(code, nameof(code));
        MaxLoadKg = Guard.Positive(maxLoadKg, nameof(maxLoadKg));
    }
    
    /// <summary>Strefa, do którego należy lokacja</summary>
    public Zone Zone { get; }
    
    /// <summary>Kod lokalizacji</summary>
    public string Code { get; }
    
    /// <summary>Maksymalne obciążenie w kg</summary>
    public decimal MaxLoadKg { get; }

    /// <summary>Sklejenie pełnego kodu w postaci MAGAZYN/STREFA/LOKALIZACJA</summary>
    public string FullCode => $"{Zone.Warehouse.Code}/{Zone.Code}/{Code}";
    
    ///<inheritdoc />
    public override string ToString() => FullCode;
}