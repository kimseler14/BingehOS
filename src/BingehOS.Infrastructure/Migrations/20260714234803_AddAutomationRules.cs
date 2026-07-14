using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAutomationRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "WorkOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AutomationRuleExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    Detail = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationRuleExecutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutomationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TriggerType = table.Column<int>(type: "integer", nullable: false),
                    ConditionJson = table.Column<string>(type: "text", nullable: false),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    ActionParametersJson = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationRules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutomationRuleExecutions_TenantId_RuleId_ExecutedAt",
                table: "AutomationRuleExecutions",
                columns: new[] { "TenantId", "RuleId", "ExecutedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AutomationRules_TenantId_TriggerType",
                table: "AutomationRules",
                columns: new[] { "TenantId", "TriggerType" });

            migrationBuilder.Sql(@"ALTER TABLE ""AutomationRules"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""AutomationRules"" FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""AutomationRuleExecutions"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""AutomationRuleExecutions"" FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""AutomationRules"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""AutomationRuleExecutions"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");

            migrationBuilder.Sql("""
                INSERT INTO "Permissions" ("Id", "TenantId", "Name", "Description", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '22222222-2222-2222-2222-222222222268', '11111111-1111-1111-1111-111111111111',
                       'automation-rules.read', 'Read automation rules and execution logs', NOW(), NOW(), false
                WHERE NOT EXISTS (
                    SELECT 1 FROM "Permissions"
                    WHERE "TenantId" = '11111111-1111-1111-1111-111111111111'
                      AND "Name" = 'automation-rules.read'
                );
                INSERT INTO "Permissions" ("Id", "TenantId", "Name", "Description", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '22222222-2222-2222-2222-222222222269', '11111111-1111-1111-1111-111111111111',
                       'automation-rules.write', 'Create, update, and delete automation rules', NOW(), NOW(), false
                WHERE NOT EXISTS (
                    SELECT 1 FROM "Permissions"
                    WHERE "TenantId" = '11111111-1111-1111-1111-111111111111'
                      AND "Name" = 'automation-rules.write'
                );
                INSERT INTO "RolePermissions" ("Id", "TenantId", "RoleId", "PermissionId", "AssignedAt", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '33333333-3333-3333-3333-333333333378', '11111111-1111-1111-1111-111111111111',
                       '11111111-1111-1111-1111-111111111111',
                       '22222222-2222-2222-2222-222222222268', NOW(), NOW(), NOW(), false
                WHERE NOT EXISTS (
                    SELECT 1 FROM "RolePermissions"
                    WHERE "TenantId" = '11111111-1111-1111-1111-111111111111'
                      AND "RoleId" = '11111111-1111-1111-1111-111111111111'
                      AND "PermissionId" = '22222222-2222-2222-2222-222222222268'
                );
                INSERT INTO "RolePermissions" ("Id", "TenantId", "RoleId", "PermissionId", "AssignedAt", "CreatedAt", "UpdatedAt", "IsDeleted")
                SELECT '33333333-3333-3333-3333-333333333379', '11111111-1111-1111-1111-111111111111',
                       '11111111-1111-1111-1111-111111111111',
                       '22222222-2222-2222-2222-222222222269', NOW(), NOW(), NOW(), false
                WHERE NOT EXISTS (
                    SELECT 1 FROM "RolePermissions"
                    WHERE "TenantId" = '11111111-1111-1111-1111-111111111111'
                      AND "RoleId" = '11111111-1111-1111-1111-111111111111'
                      AND "PermissionId" = '22222222-2222-2222-2222-222222222269'
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""AutomationRuleExecutions"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""AutomationRules"";");
            migrationBuilder.Sql(@"ALTER TABLE ""AutomationRuleExecutions"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""AutomationRuleExecutions"" NO FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""AutomationRules"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""AutomationRules"" NO FORCE ROW LEVEL SECURITY;");

            migrationBuilder.DropTable(
                name: "AutomationRuleExecutions");

            migrationBuilder.DropTable(
                name: "AutomationRules");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "WorkOrders");
        }
    }
}
