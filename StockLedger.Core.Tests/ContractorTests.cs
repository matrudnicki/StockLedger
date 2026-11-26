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

    [Fact]
    public void Constructor_ValidData_SetsProperties()
    {
        Contractor contractor = new Contractor("Hurtownia", ContractorType.Supplier, "123-456-32-18");
        
        Assert.Equal("Hurtownia", contractor.Name);
        Assert.Equal(ContractorType.Supplier, contractor.Type);
        Assert.Equal("1234563218", contractor.TaxId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_BlankName_Throws(string name)
    {
        Assert.Throws<ArgumentException>(() => new Contractor(name, ContractorType.Supplier, "1234563218"));
    }

    [Fact]
    public void Constructor_InvalidNip_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Contractor("Sklep", ContractorType.Customer, "1234563219"));
    }

    [Fact]
    public void Constructor_UnknownType_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Contractor("Sklep", (ContractorType)0, "1234563218"));
    }

    [Fact]
    public void Constructor_ValidEmail_SetsEmail()
    {
        Contractor contractor = new Contractor("Skelp", ContractorType.Customer, "1234563218", "biuro@sklep.pl");
        
        Assert.Equal("biuro@sklep.pl", contractor.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NoEmail_EmailIsNull(string? email)
    {
        Contractor contractor = new Contractor("Sklep", ContractorType.Customer, "1234563218", email);
        
        Assert.Null(contractor.Email);
    }

    [Theory]
    [InlineData("kot")]
    [InlineData("biuro @sklep.pl")]
    [InlineData("@sklep.pl")]
    [InlineData("biuro@sklep")]
    [InlineData("biuro@.pl")]
    [InlineData("biuro@sklep.")]
    [InlineData("biuro@@sklep.pl")]
    public void Constructor_InvalidEmail_Throws(string email)
    {
        Assert.Throws<ArgumentException>(() => new Contractor("Sklep", ContractorType.Customer, "1234563218", email));
    }
    
    
}