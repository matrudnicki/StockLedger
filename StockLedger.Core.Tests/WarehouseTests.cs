using Xunit;

namespace StockLedger.Core.Tests;

public class WarehouseTests
{
    public static Warehouse CreateWarehouse() => new("mag1", "Magazyn główny");
    
    [Fact]
    public void Constructor_NormalizesCode()
    {
        Assert.Equal("MAG1", CreateWarehouse().Code);
    }

    [Fact]
    public void AddZone_AddZoneLinkedToWarehouse()
    {
        var warehouse = CreateWarehouse();
        var zone = warehouse.AddZone("a", "Regały suche");
        Assert.Single(warehouse.Zones);
        Assert.Same(warehouse, zone.Warehouse);
        Assert.Equal("A", zone.Code);
    }
    
    
    [Fact]
    public void AddZone_DuplicateCode_Throws()
    {
        var warehouse = CreateWarehouse();
        var zone = warehouse.AddZone("A", "Regały suche");

        var exception = Assert.Throws<DuplicateCodeException>(() => warehouse.AddZone("a", "Inna"));
        Assert.Equal("Strefa", exception.EntityName);
        Assert.Equal("A", exception.Code);
    }

    [Fact]
    public void AddLocation_NonPositiveLoad_Throws()
    {
        var zone = CreateWarehouse().AddZone("A", "Regały suche");
        Assert.Throws<ArgumentOutOfRangeException>( () => zone.AddLocation("A-01", 0m));
    }
    
    [Fact]
    public void AddLocation_SameCodeInDiffZones_IsAllowed()
    {
        var warehouse = CreateWarehouse();
        var first = warehouse.AddZone("A",  "Inna").AddLocation("01", 100m);
        var second = warehouse.AddZone("B",  "Inna").AddLocation("01", 100m);
        
        Assert.NotSame(first.FullCode, second.FullCode);
    }
    
    [Fact]
    public void Locations_ReturnsLocationsFromAllZones()
    {
        var warehouse = CreateWarehouse();
        var zone1 = warehouse.AddZone("A", "Inna");
        var zone2 = warehouse.AddZone("B", "Inna");
        zone1.AddLocation("01", 100m);
        zone1.AddLocation("02", 100m);
        zone2.AddLocation("01", 100m);
        
        Assert.Equal(3, warehouse.Locations.Count());
    }

    [Fact]
    public void DuplicateCodeException_IsDomainException()
    {
        var exception = new DuplicateCodeException("Strefa", "A");
        
        Assert.IsAssignableFrom<DomainException>(exception);
    }
}