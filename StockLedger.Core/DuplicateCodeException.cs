namespace StockLedger.Core;

/// <summary>Prób dodania obiektu o kodzie, który już istnieje w tym kontekście</summary>
public class DuplicateCodeException : DomainException
{
    /// <summary>Tworzy wyjątek dla zduplikowanego kodu. </summary>
    /// <param name="entityName">Nazwa rodzaju obiektu, np. "Strefa."</param>
    /// <param name="code">Zduplikowany kod.</param>
    public DuplicateCodeException(string entityName, string code) : 
        base($"{entityName} with code {code} is already in use")
    {
        EntityName = entityName;
        Code = code;
    }
    
    /// <summary>Rodzaj obiektu, którego dotyczy duplikat.</summary>
    public string EntityName { get; }
    
    /// <summary> Zduplikowany kod.</summary>
    public string Code { get; }
}