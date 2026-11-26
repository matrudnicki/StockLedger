namespace StockLedger.Core;

/// <summary>Kontrahent: firma, od której przyjmujemy towar albo której go wydajemy.</summary>
public class Contractor
{
    /// <summary>Tworzy kontrahenta.</summary>
    /// <param name="name">Nazwa firmy, niepusta.</param>
    /// <param name="type">Dostawca albo odbiorca</param>
    /// <param name="taxId">NIP, obowiązkowy, z myślnikami albo bez.</param>
    /// <param name="email">Adres e-mail opcjonalny</param>
    /// <exception cref="ArgumentException">Gdy nazwa jest pusta lub NIP jest niepoprawny albo e-mail ma zły format.</exception>
    public Contractor(string name, ContractorType type, string taxId, string? email = null)
    {
        if (!IsValidNip(taxId))
        {
            throw new ArgumentException("Niepoprawny NIP.", nameof(taxId));
        }

        if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
        {
            throw new ArgumentException("Niepoprawny adres e-mail.", nameof(email));
        }
        
        Name = Guard.NotBlank(name, nameof(name));
        Type = type;
        TaxId = NormalizeNip(taxId);
        Email = string.IsNullOrWhiteSpace(email) ? null : email;
    }
    
    /// <summary>Nazwa firmy</summary>
    public string Name { get; }
    
    /// <summary>Rola kontrahenta: dostawca albo odbiorca.</summary>
    public ContractorType Type { get; }
    
    /// <summary>NIP zapisany jako same cyfry, np 1234563218</summary>
    public string TaxId { get; }
    
    /// <summary>Adres e-mail albo null, gdy nie podano.</summary>
    public string? Email { get; }
    
    /// <summary>Sprawdza, czy NIP jest poprawny: 10 cyfr i zgodna cyfra kontrolna.</summary>
    /// <param name="nip">NIP do sprawdzenia, może zawierać myślniki i spacje.</param>
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
            if (!char.IsDigit(c))
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

    private static bool IsValidEmail(string email)
    {
        if (email.Contains(' '))
        {
            return false;
        }

        int atIndex = email.IndexOf('@');

        if (atIndex <= 0 || atIndex != email.LastIndexOf('@'))
        {
            return false;
        }

        int dotIndex = email.LastIndexOf('.');

        return dotIndex > atIndex + 1 && dotIndex < email.Length - 1;

    }
    
    private static string NormalizeNip(string nip) => nip.Replace("-", "").Replace(" ", "");
}