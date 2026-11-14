namespace StockLedger.Core;

/// <summary>Metoda wyceny rozchodu zapasów</summary>
public enum ValuationType
{
    /// <summary>Rozchód najstarszych partii jako pierwszych</summary>
    Fifo,
    /// <summary>Średnia ważona ceny wszystkich dostępnych partii</summary>
    WeightedAverage
}