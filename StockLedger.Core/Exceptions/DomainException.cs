namespace StockLedger.Core;

/// <summary>Bazowy wyjątek dla naruszeń reguł biznesowych systemu.</summary>
public class DomainException : Exception
{
    /// <summary>Tworzy wyjątek z komunikatem.</summary>
    public DomainException(string message) : base(message)
    {
    }

    /// <summary>Tworzy wyjątek z komunikatem i przyczyną.</summary>
    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}