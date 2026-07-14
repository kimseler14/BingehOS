using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinanceExtensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CostCenterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    PlannedAmountMinor = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CostCenterId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    AllocatedAmountMinor = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    AllocationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostAllocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DowntimeCosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CostPerHourMinor = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DowntimeCosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnergyCosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillingPeriodStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BillingPeriodEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AmountMinor = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    EnergyType = table.Column<string>(type: "text", nullable: true),
                    MeterNumber = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    DocumentUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyCosts", x => x.Id);
                });

            migrationBuilder.Sql("ALTER TABLE \"Budgets\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"CostAllocations\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"DowntimeCosts\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"EnergyCosts\" ENABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Budgets\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"CostAllocations\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"DowntimeCosts\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"EnergyCosts\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"EnergyCosts\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"DowntimeCosts\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"CostAllocations\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Budgets\";");

            migrationBuilder.Sql("ALTER TABLE \"EnergyCosts\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"DowntimeCosts\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"CostAllocations\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Budgets\" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.DropTable(name: "CostAllocations");
            migrationBuilder.DropTable(name: "EnergyCosts");
            migrationBuilder.DropTable(name: "DowntimeCosts");
            migrationBuilder.DropTable(name: "Budgets");
        }
    }
}
