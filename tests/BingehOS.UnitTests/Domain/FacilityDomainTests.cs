using BingehOS.Modules.Facility.Domain;

namespace BingehOS.UnitTests.Domain;

public class FacilityDomainTests
{
    [Fact]
    public void Campus_Create_Rename_SetLocation()
    {
        var tenant = Guid.NewGuid();
        var campus = Campus.Create(tenant, "Main Campus", "MC", "Istanbul", "Europe/Istanbul", 41.0, 29.0);

        Assert.Equal(tenant, campus.TenantId);
        Assert.Equal("Main Campus", campus.Name);
        Assert.Equal("MC", campus.Code);
        Assert.Equal("Istanbul", campus.Address);
        Assert.Equal("Europe/Istanbul", campus.TimeZone);
        Assert.Equal(41.0, campus.Latitude);
        Assert.Equal(29.0, campus.Longitude);
        Assert.Empty(campus.Buildings);

        campus.Rename("HQ Campus");
        Assert.Equal("HQ Campus", campus.Name);

        campus.SetLocation(40.5, 28.5);
        Assert.Equal(40.5, campus.Latitude);
        Assert.Equal(28.5, campus.Longitude);
    }

    [Fact]
    public void Building_Create_Rename_SetFloorsCount()
    {
        var tenant = Guid.NewGuid();
        var campusId = Guid.NewGuid();
        var b = Building.Create(tenant, campusId, "Block A", "BA", "addr", "Europe/Istanbul", 5);

        Assert.Equal(campusId, b.CampusId);
        Assert.Equal("Block A", b.Name);
        Assert.Equal(5, b.FloorsCount);
        Assert.Empty(b.Floors);

        b.Rename("Block B");
        Assert.Equal("Block B", b.Name);

        b.SetFloorsCount(8);
        Assert.Equal(8, b.FloorsCount);
    }

    [Fact]
    public void Floor_Create_And_Rename()
    {
        var tenant = Guid.NewGuid();
        var buildingId = Guid.NewGuid();
        var floor = Floor.Create(tenant, buildingId, "Ground", "G", 0);

        Assert.Equal(buildingId, floor.BuildingId);
        Assert.Equal("Ground", floor.Name);
        Assert.Equal(0, floor.Level);
        Assert.Empty(floor.Zones);
        Assert.Empty(floor.Rooms);

        floor.Rename("Lobby Floor");
        Assert.Equal("Lobby Floor", floor.Name);
    }

    [Fact]
    public void Zone_Create_Defaults_And_Rename()
    {
        var tenant = Guid.NewGuid();
        var floorId = Guid.NewGuid();
        var zone = Zone.Create(tenant, floorId, "North Wing", "NW", SpatialType.Area);

        Assert.Equal(floorId, zone.FloorId);
        Assert.Equal("North Wing", zone.Name);
        Assert.Equal(SpatialType.Area, zone.Type);
        Assert.Empty(zone.Rooms);

        zone.Rename("South Wing");
        Assert.Equal("South Wing", zone.Name);
    }

    [Fact]
    public void Room_Create_With_Floor_And_Zone()
    {
        var tenant = Guid.NewGuid();
        var floorId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();
        var room = Room.Create(tenant, floorId, zoneId, "Server Room", "SR-1", SpatialType.Room, "d");

        Assert.Equal(floorId, room.FloorId);
        Assert.Equal(zoneId, room.ZoneId);
        Assert.Equal("Server Room", room.Name);
        Assert.Equal(SpatialType.Room, room.Type);
        Assert.Equal("d", room.Description);

        room.Rename("Data Room");
        Assert.Equal("Data Room", room.Name);
    }

    [Fact]
    public void Room_Create_Allows_Null_Floor_And_Zone()
    {
        var room = Room.Create(Guid.NewGuid(), null, null, "Outdoor", null, SpatialType.Outdoor, null);
        Assert.Null(room.FloorId);
        Assert.Null(room.ZoneId);
        Assert.Equal(SpatialType.Outdoor, room.Type);
    }
}
