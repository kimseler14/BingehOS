using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInsightsPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO ""Permissions"" (""Id"", ""TenantId"", ""Name"", ""Description"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                SELECT '22222222-2222-2222-2222-222222222272', '11111111-1111-1111-1111-111111111111',
                       'insights.read', 'Read statistical maintenance and inventory insights', NOW(), NOW(), false
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""Permissions""
                    WHERE ""TenantId"" = '11111111-1111-1111-1111-111111111111'
                      AND ""Name"" = 'insights.read'
                );
                INSERT INTO ""RolePermissions"" (""Id"", ""TenantId"", ""RoleId"", ""PermissionId"", ""AssignedAt"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                SELECT '33333333-3333-3333-3333-333333333382', '11111111-1111-1111-1111-111111111111',
                       '11111111-1111-1111-1111-111111111111',
                       '22222222-2222-2222-2222-222222222272', NOW(), NOW(), NOW(), false
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""RolePermissions""
                    WHERE ""TenantId"" = '11111111-1111-1111-1111-111111111111'
                      AND ""RoleId"" = '11111111-1111-1111-1111-111111111111'
                      AND ""PermissionId"" = '22222222-2222-2222-2222-222222222272'
                );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM ""RolePermissions""
                WHERE ""TenantId"" = '11111111-1111-1111-1111-111111111111'
                  AND ""PermissionId"" = (
                      SELECT ""Id""
                      FROM ""Permissions""
                      WHERE ""TenantId"" = '11111111-1111-1111-1111-111111111111'
                        AND ""Name"" = 'insights.read'
                  );
                DELETE FROM ""Permissions""
                WHERE ""TenantId"" = '11111111-1111-1111-1111-111111111111'
                  AND ""Name"" = 'insights.read';");
        }
    }
}
