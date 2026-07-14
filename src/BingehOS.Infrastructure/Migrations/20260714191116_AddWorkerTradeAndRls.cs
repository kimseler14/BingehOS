using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerTradeAndRls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    EmployeeNumber = table.Column<string>(type: "text", nullable: true),
                    Trade = table.Column<string>(type: "text", nullable: true),
                    Department = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.Id);
                });

            migrationBuilder.Sql(@"ALTER TABLE ""Workers"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Workers"" FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""Workers"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""Workers"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Workers"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Workers"" NO FORCE ROW LEVEL SECURITY;");
            migrationBuilder.DropTable(
                name: "Workers");
        }
    }
}
