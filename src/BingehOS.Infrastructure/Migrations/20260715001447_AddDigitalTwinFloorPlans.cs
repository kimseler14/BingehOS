using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalTwinFloorPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    FloorPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: false),
                    Y = table.Column<double>(type: "double precision", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FloorPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FloorPlans", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetPositions_TenantId_FloorPlanId_AssetId",
                table: "AssetPositions",
                columns: new[] { "TenantId", "FloorPlanId", "AssetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FloorPlans_TenantId_FacilityId",
                table: "FloorPlans",
                columns: new[] { "TenantId", "FacilityId" });

            migrationBuilder.Sql(@"ALTER TABLE ""AssetPositions"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""AssetPositions"" FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""FloorPlans"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""FloorPlans"" FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""AssetPositions"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""FloorPlans"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql("""
                INSERT INTO "Permissions" ("Id", "TenantId", "Name", "Description", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '22222222-2222-2222-2222-222222222273', '11111111-1111-1111-1111-111111111111',
                       'floor-plans.read', 'Read digital twin floor plans and asset positions', NOW(), NOW(), false
                WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "TenantId" = '11111111-1111-1111-1111-111111111111' AND "Name" = 'floor-plans.read');
                INSERT INTO "Permissions" ("Id", "TenantId", "Name", "Description", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '22222222-2222-2222-2222-222222222274', '11111111-1111-1111-1111-111111111111',
                       'floor-plans.write', 'Create, update, and delete floor plans and asset positions', NOW(), NOW(), false
                WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "TenantId" = '11111111-1111-1111-1111-111111111111' AND "Name" = 'floor-plans.write');
                INSERT INTO "RolePermissions" ("Id", "TenantId", "RoleId", "PermissionId", "AssignedAt", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '33333333-3333-3333-3333-333333333383', '11111111-1111-1111-1111-111111111111',
                       '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222273', NOW(), NOW(), NOW(), false
                WHERE NOT EXISTS (SELECT 1 FROM "RolePermissions" WHERE "TenantId" = '11111111-1111-1111-1111-111111111111' AND "RoleId" = '11111111-1111-1111-1111-111111111111' AND "PermissionId" = '22222222-2222-2222-2222-222222222273');
                INSERT INTO "RolePermissions" ("Id", "TenantId", "RoleId", "PermissionId", "AssignedAt", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '33333333-3333-3333-3333-333333333384', '11111111-1111-1111-1111-111111111111',
                       '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222274', NOW(), NOW(), NOW(), false
                WHERE NOT EXISTS (SELECT 1 FROM "RolePermissions" WHERE "TenantId" = '11111111-1111-1111-1111-111111111111' AND "RoleId" = '11111111-1111-1111-1111-111111111111' AND "PermissionId" = '22222222-2222-2222-2222-222222222274');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""AssetPositions"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""FloorPlans"";");
            migrationBuilder.Sql(@"ALTER TABLE ""AssetPositions"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""AssetPositions"" NO FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""FloorPlans"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""FloorPlans"" NO FORCE ROW LEVEL SECURITY;");
            migrationBuilder.DropTable(
                name: "AssetPositions");

            migrationBuilder.DropTable(
                name: "FloorPlans");
        }
    }
}
