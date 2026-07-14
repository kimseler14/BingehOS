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
                    ('22222222-2222-2222-2222-222222222223', '11111111-1111-1111-1111-111111111111', 'assets.write', 'Write assets', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222224', '11111111-1111-1111-1111-111111111111', 'facilities.read', 'Read facilities', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222225', '11111111-1111-1111-1111-111111111111', 'facilities.write', 'Write facilities', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222226', '11111111-1111-1111-1111-111111111111', 'work-orders.read', 'Read work orders', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222227', '11111111-1111-1111-1111-111111111111', 'work-orders.write', 'Write work orders', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222228', '11111111-1111-1111-1111-111111111111', 'employees.read', 'Read employees', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222229', '11111111-1111-1111-1111-111111111111', 'employees.write', 'Write employees', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222230', '11111111-1111-1111-1111-111111111111', 'sgk-records.read', 'Read SGK records', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222231', '11111111-1111-1111-1111-111111111111', 'sgk-records.write', 'Write SGK records', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222232', '11111111-1111-1111-1111-111111111111', 'subcontractors.read', 'Read subcontractors', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222233', '11111111-1111-1111-1111-111111111111', 'subcontractors.write', 'Write subcontractors', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222234', '11111111-1111-1111-1111-111111111111', 'invoices.read', 'Read invoices', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222235', '11111111-1111-1111-1111-111111111111', 'invoices.write', 'Write invoices', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222236', '11111111-1111-1111-1111-111111111111', 'tax-records.read', 'Read tax records', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222237', '11111111-1111-1111-1111-111111111111', 'tax-records.write', 'Write tax records', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222238', '11111111-1111-1111-1111-111111111111', 'cost-centers.read', 'Read cost centers', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222239', '11111111-1111-1111-1111-111111111111', 'cost-centers.write', 'Write cost centers', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222240', '11111111-1111-1111-1111-111111111111', 'compliance-records.read', 'Read compliance records', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222241', '11111111-1111-1111-1111-111111111111', 'compliance-records.write', 'Write compliance records', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222242', '11111111-1111-1111-1111-111111111111', 'kvkk-consents.read', 'Read KVKK consents', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222243', '11111111-1111-1111-1111-111111111111', 'kvkk-consents.write', 'Write KVKK consents', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222244', '11111111-1111-1111-1111-111111111111', 'calibration-records.read', 'Read calibration records', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222245', '11111111-1111-1111-1111-111111111111', 'calibration-records.write', 'Write calibration records', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222246', '11111111-1111-1111-1111-111111111111', 'permits.read', 'Read permits', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222247', '11111111-1111-1111-1111-111111111111', 'permits.write', 'Write permits', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222248', '11111111-1111-1111-1111-111111111111', 'risk-assessments.read', 'Read risk assessments', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222249', '11111111-1111-1111-1111-111111111111', 'risk-assessments.write', 'Write risk assessments', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222250', '11111111-1111-1111-1111-111111111111', 'loto-procedures.read', 'Read LOTO procedures', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222251', '11111111-1111-1111-1111-111111111111', 'loto-procedures.write', 'Write LOTO procedures', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222252', '11111111-1111-1111-1111-111111111111', 'parts.read', 'Read parts', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222253', '11111111-1111-1111-1111-111111111111', 'parts.write', 'Write parts', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222254', '11111111-1111-1111-1111-111111111111', 'vendors.read', 'Read vendors', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222255', '11111111-1111-1111-1111-111111111111', 'vendors.write', 'Write vendors', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222256', '11111111-1111-1111-1111-111111111111', 'purchase-requests.read', 'Read purchase requests', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222257', '11111111-1111-1111-1111-111111111111', 'purchase-requests.write', 'Write purchase requests', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222258', '11111111-1111-1111-1111-111111111111', 'purchase-orders.read', 'Read purchase orders', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222259', '11111111-1111-1111-1111-111111111111', 'purchase-orders.write', 'Write purchase orders', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222260', '11111111-1111-1111-1111-111111111111', 'contracts.read', 'Read contracts', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222261', '11111111-1111-1111-1111-111111111111', 'contracts.write', 'Write contracts', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222262', '11111111-1111-1111-1111-111111111111', 'warehouses.read', 'Read warehouses', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222263', '11111111-1111-1111-1111-111111111111', 'warehouses.write', 'Write warehouses', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222264', '11111111-1111-1111-1111-111111111111', 'inventory-transactions.read', 'Read inventory transactions', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222265', '11111111-1111-1111-1111-111111111111', 'inventory-transactions.write', 'Write inventory transactions', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222266', '11111111-1111-1111-1111-111111111111', 'job-plan-templates.read', 'Read job plan templates', NOW(), NOW(), false),
                    ('22222222-2222-2222-2222-222222222267', '11111111-1111-1111-1111-111111111111', 'job-plan-templates.write', 'Write job plan templates', NOW(), NOW(), false)
                ON CONFLICT DO NOTHING;");

            migrationBuilder.Sql(@"
                INSERT INTO ""RolePermissions"" (""Id"", ""TenantId"", ""RoleId"", ""PermissionId"", ""AssignedAt"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                VALUES
                    ('33333333-3333-3333-3333-333333333331', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222221', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333332', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333333', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222223', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333334', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222224', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333335', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222225', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333336', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222226', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333337', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222227', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333338', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222228', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333339', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222229', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333340', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222230', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333341', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222231', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333342', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222232', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333343', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222233', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333344', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222234', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333345', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222235', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333346', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222236', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333347', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222237', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333348', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222238', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333349', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222239', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333350', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222240', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333351', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222241', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333352', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222242', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333353', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222243', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333354', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222244', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333355', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222245', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333356', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222246', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333357', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222247', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333358', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222248', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333359', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222249', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333360', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222250', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333361', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222251', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333362', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222252', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333363', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222253', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333364', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222254', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333365', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222255', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333366', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222256', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333367', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222257', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333368', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222258', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333369', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222259', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333370', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222260', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333371', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222261', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333372', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222262', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333373', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222263', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333374', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222264', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333375', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222265', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333376', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222266', NOW(), NOW(), NOW(), false),
                    ('33333333-3333-3333-3333-333333333377', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222267', NOW(), NOW(), NOW(), false)
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
