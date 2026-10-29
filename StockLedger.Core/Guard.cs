namespace StockLedger.Core;

internal static class Guard
{
    public static string NotBlank(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty", paramName);
        }

        return value.Trim();
    }

    public static decimal Positive(decimal value, string paramName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be greater than zero");
        }
        
        return value;
    }
}