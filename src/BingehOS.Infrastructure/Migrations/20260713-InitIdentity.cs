using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AuthProvider = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey("FK_UserRoles_Roles_RoleId", x => x.RoleId, "Roles", "Id");
                    table.ForeignKey("FK_UserRoles_Users_UserId", x => x.UserId, "Users", "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey("FK_RolePermissions_Permissions_PermissionId", x => x.PermissionId, "Permissions", "Id");
                    table.ForeignKey("FK_RolePermissions_Roles_RoleId", x => x.RoleId, "Roles", "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Email",
                table: "Users",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId_Name",
                table: "Roles",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_TenantId_Name",
                table: "Permissions",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TenantId_UserId_RoleId",
                table: "UserRoles",
                columns: new[] { "TenantId", "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_TenantId_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "TenantId", "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.Sql("ALTER TABLE \"Users\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Roles\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Permissions\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"UserRoles\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"RolePermissions\" ENABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Users\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Roles\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Permissions\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"UserRoles\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"RolePermissions\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");

            migrationBuilder.Sql(@"
                INSERT INTO ""Roles"" (""Id"", ""TenantId"", ""Name"", ""Description"", ""IsSystem"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                VALUES ('11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'SystemAdmin', 'System-wide administrator', true, NOW(), NOW(), false)
                ON CONFLICT DO NOTHING;");

            migrationBuilder.Sql(@"
                INSERT INTO ""Permissions"" (""Id"", ""TenantId"", ""Name"", ""Description"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                VALUES
                    ('22222222-2222-2222-2222-222222222221', '11111111-1111-1111-1111-111111111111', 'admin.access', 'Access admin panel', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222222', '11111111-1111-1111-1111-111111111111', 'assets.read', 'Read assets', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222223', '11111111-1111-1111-1111-111111111111', 'assets.write', 'Write assets', NOW(), NOW(), false)
                ON CONFLICT DO NOTHING;");

            migrationBuilder.Sql(@"
                INSERT INTO ""RolePermissions"" (""Id"", ""TenantId"", ""RoleId"", ""PermissionId"", ""AssignedAt"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                VALUES
                    ('33333333-3333-3333-3333-333333333331', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222221', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333332', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333333', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222223', NOW(), NOW(), NOW(), false)
                ON CONFLICT DO NOTHING;");

            migrationBuilder.Sql(@"
                INSERT INTO ""Users"" (""Id"", ""TenantId"", ""Email"", ""PasswordHash"", ""FullName"", ""IsActive"", ""AuthProvider"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                 VALUES ('44444444-4444-4444-4444-444444444444', '11111111-1111-1111-1111-111111111111', 'admin@system', '$2a$11$qmSMcSD.dKSuejhVIC2t1eDhKvCT./.YqhEVR5yW5Y9AayOaJQ3mu', 'System Admin', true, 0, NOW(), NOW(), false)
                ON CONFLICT DO NOTHING;");

            migrationBuilder.Sql(@"
                INSERT INTO ""UserRoles"" (""Id"", ""TenantId"", ""UserId"", ""RoleId"", ""AssignedAt"", ""AssignedByUserId"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                VALUES ('55555555-5555-5555-5555-555555555555', '11111111-1111-1111-1111-111111111111', '44444444-4444-4444-4444-444444444444', '11111111-1111-1111-1111-111111111111', NOW(), '44444444-4444-4444-4444-444444444444', NOW(), NOW(), false)
                ON CONFLICT DO NOTHING;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""RolePermissions"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""UserRoles"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""Permissions"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""Roles"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""Users"";");

            migrationBuilder.Sql(@"ALTER TABLE ""RolePermissions"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""UserRoles"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Permissions"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Roles"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.DropTable(name: "RolePermissions");
            migrationBuilder.DropTable(name: "UserRoles");
            migrationBuilder.DropTable(name: "Permissions");
            migrationBuilder.DropTable(name: "Roles");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
