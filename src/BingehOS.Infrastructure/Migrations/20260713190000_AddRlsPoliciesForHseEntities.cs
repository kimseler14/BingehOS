using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRlsPoliciesForHseEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""PermitsToWork"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""RiskAssessments"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""LotoProcedures"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""PermitsToWork"" FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""RiskAssessments"" FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""LotoProcedures"" FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""PermitsToWork"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""RiskAssessments"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""LotoProcedures"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""LotoProcedures"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""RiskAssessments"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""PermitsToWork"";");
            migrationBuilder.Sql(@"ALTER TABLE ""LotoProcedures"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""RiskAssessments"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""PermitsToWork"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""LotoProcedures"" NO FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""RiskAssessments"" NO FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""PermitsToWork"" NO FORCE ROW LEVEL SECURITY;");
        }
    }
}
