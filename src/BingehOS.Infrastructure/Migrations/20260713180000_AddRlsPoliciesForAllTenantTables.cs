using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRlsPoliciesForAllTenantTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""WorkOrders"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Assets"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Facilities"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Parts"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Vendors"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Incidents"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Workers"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""WorkOrderCosts"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""ComplianceRecords"" ENABLE ROW LEVEL SECURITY;");

            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""WorkOrders"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""Assets"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""Facilities"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""Parts"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""Vendors"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""Incidents"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""Workers"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""WorkOrderCosts"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""ComplianceRecords"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""WorkOrders"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""Assets"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""Facilities"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""Parts"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""Vendors"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""Incidents"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""Workers"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""WorkOrderCosts"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""ComplianceRecords"";");

            migrationBuilder.Sql(@"ALTER TABLE ""WorkOrders"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Assets"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Facilities"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Parts"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Vendors"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Incidents"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""Workers"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""WorkOrderCosts"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""ComplianceRecords"" DISABLE ROW LEVEL SECURITY;");
        }
    }
}
