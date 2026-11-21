namespace StockLedger.Core;

internal static class Guard
{
    public static string NotBlank(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Wartość nie może być pusta.", paramName);
        }

        return value.Trim();
    }

    public static decimal Positive(decimal value, string paramName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, "Wartość musi być większa od zera.");
        }
        
        return value;
    }

    public static string Code(string? value, string paramName) => NotBlank(value, paramName).ToUpperInvariant();
    
}