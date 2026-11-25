namespace StockLedger.Core;

/// <summary>Kontrahent: firma, od której przyjmujemy towar albo której go wydajemy.</summary>
public class Contractor
{
    /// <summary>Tworzy kontrahenta.</summary>
    /// <param name="name">Nazwa firmy, niepusta.</param>
    /// <param name="type">Dostawca albo odbiorca</param>
    /// <param name="taxId">NIP, obowiązkowy, z myślnikami albo bez.</param>
    /// <exception cref="ArgumentException">Gdy nazwa jest pusta lub NIP jest niepoprawny.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Gdy typ nie jest znaną wartością.</exception>
    public Contractor(string name, ContractorType type, string taxId)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown contractor type.");
        }

        if (!IsValidNip(taxId))
        {
            throw new ArgumentException("Invalid tax id (NIP).", nameof(taxId));
        }
        
        Name = Guard.NotBlank(name, nameof(name));
        Type = type;
        TaxId = NormalizeNip(taxId);
    }
    
    /// <summary>Nazwa firmy</summary>
    public string Name { get; }
    
    /// <summary>Rola kontrahenta: dostawca albo odbiorca.</summary>
    public ContractorType Type { get; }
    
    /// <summary>NIP zapisany jako same cyfry, np 1234563218</summary>
    public string TaxId { get; }
    
    /// <summary>Sprawdza, czy NIP jest poprawny: 10 cyfr i zgodna cyfra kontrolna.</summary>
    /// <param name="nip">NIP do sprawdzenia, może zawierać myćlniki i spacje.</param>
    /// <returns>true, gdy NIP jest poprawny, inaczej false.</returns>
    public static bool IsValidNip(string? nip)
    {
        if (nip is null)
        {
            return false;
        }

        string digits = NormalizeNip(nip);

        if (digits.Length != 10 || !HasOnlyDigits(digits))
        {
            return false;
        }
        
        int checkDigit = CalculateCheckDigit(digits);
        int lastDigit = digits[9] - '0';

        return checkDigit == lastDigit;
        
    }

    private static bool HasOnlyDigits(string text)
    {
        foreach (char c in text)
        {
            if (c < '0' || c > '9')
            {
                return false;
            }
        }

        return true;
        
    }

    private static int CalculateCheckDigit(string digits)
    {
        int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7 };
        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            sum += (digits[i] - '0') * weights[i];
        }

        return sum % 11;
    }

    private static string NormalizeNip(string nip) => nip.Replace("-", "").Replace(" ", "");
}