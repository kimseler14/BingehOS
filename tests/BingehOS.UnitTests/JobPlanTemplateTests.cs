using BingehOS.Modules.Maintenance.Domain;
using Xunit;

namespace BingehOS.UnitTests;

public class JobPlanTemplateTests
{
    [Fact]
    public void Create_SetsAllProperties()
    {
        var template = JobPlanTemplate.Create(
            Guid.NewGuid(), "Elevator PM", "Monthly elevator maintenance", "Elevator",
            "[\"Inspect hydraulic fluid\",\"Test brakes\"]", 120, "Hard hat, Safety shoes", "Hot Work", "Hydraulic filter, Oil");

        Assert.Equal("Elevator PM", template.Name);
        Assert.Equal("Elevator", template.AssetType);
        Assert.Equal(120, template.EstimatedDurationMinutes);
        Assert.Equal("Hot Work", template.RequiredPermitType);
    }

    [Fact]
    public void Update_ChangesProperties()
    {
        var template = JobPlanTemplate.Create(Guid.NewGuid(), "Old", "Old desc", "HVAC", "[]", 60, "PPE", "Permit", "Parts");
        template.Update("New", "New desc", "Generator", "[\"Test battery\"]", 90, "New PPE", "New Permit", "New Parts");

        Assert.Equal("New", template.Name);
        Assert.Equal("Generator", template.AssetType);
        Assert.Equal(90, template.EstimatedDurationMinutes);
    }
}
