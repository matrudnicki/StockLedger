namespace StockLedger.Core;

/// <summary>Kontrahent: firma, od której przyjmujemy towar albo której go wydajemy.</summary>
public class Contractor
{
    /// <summary>Sprawdza, czy NIP jest poprawny: 10 cyfr i zgodna cyfra kontrolna.</summary>
    /// <param name="nip">NIP do sprawdzenia, może zawierać myćlniki i spacje.</param>
    /// <returns>true, gdy NIP jest poprawny, inaczej false.</returns>
    public static bool IsValidNip(string? nip)
    {
        if (nip is null)
        {
            return false;
        }

        string digits = nip.Replace("-", "").Replace(" ", "");

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
}