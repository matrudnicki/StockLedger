using Xunit;

namespace StockLedger.Core.Tests;

public class ContractorTests
{
    [Theory]
    [InlineData("1234563218")]
    [InlineData("5260250995")]
    [InlineData("9876543210")]
    [InlineData("1111111111")]
    [InlineData("123-456-32-18")]
    public void IsValidNip_CorrectNip_ReturnsTrue(string nip)
    {
        bool result = Contractor.IsValidNip(nip);

        Assert.True(result);
    }
    
    
    [Theory]
    [InlineData("1234563219")]
    [InlineData("1234567890")]
    [InlineData("12345")]
    [InlineData("abcdefghij")]
    [InlineData("")]
    [InlineData(null)]
    public void IsValidNip_IncorrectNip_ReturnsFalse(string? nip)
    {
        bool result = Contractor.IsValidNip(nip);

        Assert.False(result);
    }
    
}