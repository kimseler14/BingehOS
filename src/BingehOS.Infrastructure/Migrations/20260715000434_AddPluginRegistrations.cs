using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPluginRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PluginRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SourceUrl = table.Column<string>(type: "text", nullable: true),
                    StoragePath = table.Column<string>(type: "text", nullable: true),
                    InstalledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginRegistrations", x => x.Id);
                });

            migrationBuilder.Sql(@"ALTER TABLE ""PluginRegistrations"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""PluginRegistrations"" FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""PluginRegistrations"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");

            migrationBuilder.Sql("""
                INSERT INTO "Permissions" ("Id", "TenantId", "Name", "Description", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '22222222-2222-2222-2222-222222222270', '11111111-1111-1111-1111-111111111111',
                       'plugins.read', 'Read plugin registrations', NOW(), NOW(), false
                WHERE NOT EXISTS (
                    SELECT 1 FROM "Permissions"
                    WHERE "TenantId" = '11111111-1111-1111-1111-111111111111'
                      AND "Name" = 'plugins.read'
                );
                INSERT INTO "Permissions" ("Id", "TenantId", "Name", "Description", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '22222222-2222-2222-2222-222222222271', '11111111-1111-1111-1111-111111111111',
                       'plugins.write', 'Register, update, and delete plugin registrations', NOW(), NOW(), false
                WHERE NOT EXISTS (
                    SELECT 1 FROM "Permissions"
                    WHERE "TenantId" = '11111111-1111-1111-1111-111111111111'
                      AND "Name" = 'plugins.write'
                );
                INSERT INTO "RolePermissions" ("Id", "TenantId", "RoleId", "PermissionId", "AssignedAt", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '33333333-3333-3333-3333-333333333380', '11111111-1111-1111-1111-111111111111',
                       '11111111-1111-1111-1111-111111111111',
                       '22222222-2222-2222-2222-222222222270', NOW(), NOW(), NOW(), false
                WHERE NOT EXISTS (
                    SELECT 1 FROM "RolePermissions"
                    WHERE "TenantId" = '11111111-1111-1111-1111-111111111111'
                      AND "RoleId" = '11111111-1111-1111-1111-111111111111'
                      AND "PermissionId" = '22222222-2222-2222-2222-222222222270'
                );
                INSERT INTO "RolePermissions" ("Id", "TenantId", "RoleId", "PermissionId", "AssignedAt", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '33333333-3333-3333-3333-333333333381', '11111111-1111-1111-1111-111111111111',
                       '11111111-1111-1111-1111-111111111111',
                       '22222222-2222-2222-2222-222222222271', NOW(), NOW(), NOW(), false
                WHERE NOT EXISTS (
                    SELECT 1 FROM "RolePermissions"
                    WHERE "TenantId" = '11111111-1111-1111-1111-111111111111'
                      AND "RoleId" = '11111111-1111-1111-1111-111111111111'
                      AND "PermissionId" = '22222222-2222-2222-2222-222222222271'
                );
                """);

            migrationBuilder.CreateIndex(
                name: "IX_PluginRegistrations_TenantId_Status",
                table: "PluginRegistrations",
                columns: new[] { "TenantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""PluginRegistrations"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PluginRegistrations"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""PluginRegistrations"" NO FORCE ROW LEVEL SECURITY;");

            migrationBuilder.DropTable(
                name: "PluginRegistrations");
        }
    }
}
