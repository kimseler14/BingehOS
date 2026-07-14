using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacilityAndAssetExtensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    CampusId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    FloorsCount = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                    table.ForeignKey("FK_Buildings_Campuses_CampusId", x => x.CampusId, "Campuses", "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AssetClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: true),
                    Manufacturer = table.Column<string>(type: "text", nullable: true),
                    ExpectedLifespanYears = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.Id);
                    table.ForeignKey("FK_AssetTypes_AssetClasses_AssetClassId", x => x.AssetClassId, "AssetClasses", "Id");
                });

            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floors", x => x.Id);
                    table.ForeignKey("FK_Floors_Buildings_BuildingId", x => x.BuildingId, "Buildings", "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AssetTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Attributes = table.Column<string>(type: "text", nullable: true),
                    IsPreseeded = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTemplates", x => x.Id);
                    table.ForeignKey("FK_AssetTemplates_AssetTypes_AssetTypeId", x => x.AssetTypeId, "AssetTypes", "Id");
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    FloorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                    table.ForeignKey("FK_Zones_Floors_FloorId", x => x.FloorId, "Floors", "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetHealthScores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    CalculationDetails = table.Column<string>(type: "text", nullable: true),
                    CalculatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetHealthScores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetRelationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentAssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildAssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelationshipType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetRelationships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    MeterType = table.Column<string>(type: "text", nullable: true),
                    LastReadingAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastReadingValue = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    FloorId = table.Column<Guid>(type: "uuid", nullable: true),
                    ZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey("FK_Rooms_Floors_FloorId", x => x.FloorId, "Floors", "Id");
                    table.ForeignKey("FK_Rooms_Zones_ZoneId", x => x.ZoneId, "Zones", "Id");
                });

            migrationBuilder.CreateTable(
                name: "Warranties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DocumentUrl = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warranties", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_AssetClassId",
                table: "AssetTypes",
                column: "AssetClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTemplates_AssetTypeId",
                table: "AssetTemplates",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_CampusId",
                table: "Buildings",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Floors_BuildingId",
                table: "Floors",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_FloorId",
                table: "Zones",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_FloorId",
                table: "Rooms",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ZoneId",
                table: "Rooms",
                column: "ZoneId");

            migrationBuilder.Sql("ALTER TABLE \"Campuses\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Buildings\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Floors\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Zones\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Rooms\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"AssetClasses\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"AssetTypes\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"AssetTemplates\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Meters\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"AssetRelationships\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"AssetHealthScores\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Warranties\" ENABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Campuses\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Buildings\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Floors\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Zones\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Rooms\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"AssetClasses\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"AssetTypes\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"AssetTemplates\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Meters\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"AssetRelationships\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"AssetHealthScores\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Warranties\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Warranties\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"AssetHealthScores\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"AssetRelationships\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Meters\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"AssetTemplates\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"AssetTypes\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"AssetClasses\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Rooms\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Zones\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Floors\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Buildings\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Campuses\";");

            migrationBuilder.Sql("ALTER TABLE \"Warranties\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"AssetHealthScores\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"AssetRelationships\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Meters\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"AssetTemplates\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"AssetTypes\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"AssetClasses\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Rooms\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Zones\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Floors\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Buildings\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Campuses\" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.DropTable(name: "Warranties");
            migrationBuilder.DropTable(name: "AssetHealthScores");
            migrationBuilder.DropTable(name: "AssetRelationships");
            migrationBuilder.DropTable(name: "Meters");
            migrationBuilder.DropTable(name: "AssetTemplates");
            migrationBuilder.DropTable(name: "Rooms");
            migrationBuilder.DropTable(name: "Zones");
            migrationBuilder.DropTable(name: "Floors");
            migrationBuilder.DropTable(name: "Buildings");
            migrationBuilder.DropTable(name: "Campuses");
            migrationBuilder.DropTable(name: "AssetTypes");
            migrationBuilder.DropTable(name: "AssetClasses");
        }
    }
}
