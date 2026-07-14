using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryExtensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    ManagerUserId = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractNumber = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Terms = table.Column<string>(type: "text", nullable: true),
                    DocumentUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey("FK_Locations_Warehouses_WarehouseId", x => x.WarehouseId, "Warehouses", "Id");
                });

            migrationBuilder.CreateTable(
                name: "Shelves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shelves", x => x.Id);
                    table.ForeignKey("FK_Shelves_Locations_LocationId", x => x.LocationId, "Locations", "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestNumber = table.Column<string>(type: "text", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    ShelfId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaxCapacity = table.Column<double>(type: "double precision", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bins", x => x.Id);
                    table.ForeignKey("FK_Bins_Shelves_ShelfId", x => x.ShelfId, "Shelves", "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "text", nullable: false),
                    PurchaseRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey("FK_PurchaseOrders_PurchaseRequests_PurchaseRequestId", x => x.PurchaseRequestId, "PurchaseRequests", "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartId = table.Column<Guid>(type: "uuid", nullable: false),
                    BinId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "text", nullable: false),
                    RelatedWorkOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    RelatedPurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    TransactionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransactions", x => x.Id);
                    table.ForeignKey("FK_InventoryTransactions_Bins_BinId", x => x.BinId, "Bins", "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_WarehouseId",
                table: "Locations",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Shelves_LocationId",
                table: "Shelves",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bins_ShelfId",
                table: "Bins",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PurchaseRequestId",
                table: "PurchaseOrders",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_BinId",
                table: "InventoryTransactions",
                column: "BinId");

            migrationBuilder.Sql("ALTER TABLE \"Warehouses\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Locations\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Shelves\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Bins\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"InventoryTransactions\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"PurchaseRequests\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"PurchaseOrders\" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Contracts\" ENABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Warehouses\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Locations\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Shelves\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Bins\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"InventoryTransactions\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"PurchaseRequests\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"PurchaseOrders\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
            migrationBuilder.Sql("CREATE POLICY \"tenant_isolation\" ON \"Contracts\" USING (\"TenantId\" = current_setting('app.current_tenant_id')::uuid);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Contracts\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"PurchaseOrders\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"PurchaseRequests\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"InventoryTransactions\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Bins\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Shelves\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Locations\";");
            migrationBuilder.Sql("DROP POLICY IF EXISTS \"tenant_isolation\" ON \"Warehouses\";");

            migrationBuilder.Sql("ALTER TABLE \"Contracts\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"PurchaseOrders\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"PurchaseRequests\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"InventoryTransactions\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Bins\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Shelves\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Locations\" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE \"Warehouses\" DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.DropTable(name: "InventoryTransactions");
            migrationBuilder.DropTable(name: "PurchaseOrders");
            migrationBuilder.DropTable(name: "Bins");
            migrationBuilder.DropTable(name: "PurchaseRequests");
            migrationBuilder.DropTable(name: "Shelves");
            migrationBuilder.DropTable(name: "Locations");
            migrationBuilder.DropTable(name: "Warehouses");
            migrationBuilder.DropTable(name: "Contracts");
        }
    }
}
