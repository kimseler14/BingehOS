using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCalibrationRecordsAndTurkeyJobPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalibrationRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    CalibratedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    NextDueAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Result = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalibrationRecords", x => x.Id);
                });

            migrationBuilder.Sql("""
                WITH tenants AS (
                    SELECT DISTINCT "TenantId" FROM "Users" WHERE NOT "IsDeleted"
                    UNION
                    SELECT DISTINCT "TenantId" FROM "Roles" WHERE NOT "IsDeleted"
                ),
                templates (
                    "Name",
                    "Description",
                    "AssetType",
                    "Steps",
                    "EstimatedDurationMinutes",
                    "RequiredPpe",
                    "RequiredPermitType",
                    "RecommendedParts"
                ) AS (
                    VALUES
                    (
                        'Asansör Aylık Bakım',
                        'Asansörün aylık periyodik bakım ve güvenlik kontrolü.',
                        'Asansör',
                        '1. Kabin, kapılar ve kat seviyeleme sistemini kontrol et.
                2. Halatları, kasnağı ve fren tertibatını görsel olarak incele.
                3. Kumanda panosu, acil durum haberleşmesi ve alarmı test et.
                4. Kılavuz rayları ve hareketli parçaları temizle ve yağla.
                5. Güvenlik devrelerini ve sınır kesicilerini test et.
                6. Bulguları kayıt altına al ve bakım etiketini güncelle.',
                        120,
                        'Baret, iş ayakkabısı, eldiven, gözlük',
                        'Yüksekte Çalışma İzni',
                        'Genel bakım yağı, temizlik bezi, kablo bağı'
                    ),
                    (
                        'Yangın Söndürme Sistemleri Periyodik Bakımı',
                        'Yangın söndürme ekipmanlarının periyodik kontrol ve bakım planı.',
                        'Yangın söndürme sistemleri',
                        '1. Yangın tüplerinin mühür, basınç ve son kullanma tarihlerini kontrol et.
                2. Yangın dolapları, hortumlar ve lansların erişilebilirliğini incele.
                3. Sprinkler vanaları, göstergeler ve alarm bağlantılarını kontrol et.
                4. Tüplerin ve dolapların fiziksel durumunu temizle ve doğrula.
                5. Eksik veya hasarlı ekipmanı bildir ve etiketleri yenile.
                6. Kontrol sonuçlarını ve uygunsuzlukları raporla.',
                        90,
                        'Baret, iş ayakkabısı, eldiven, gözlük',
                        'Sıcak Çalışma İzni',
                        'Yangın tüpü pimi, mühür, etiket, hortum contası'
                    ),
                    (
                        'Jeneratör Aylık Bakım',
                        'Jeneratör setinin aylık çalıştırma ve önleyici bakım planı.',
                        'Jeneratör',
                        '1. Motor yağı, soğutma suyu ve yakıt seviyelerini kontrol et.
                2. Akü, şarj cihazı ve bağlantı terminallerini incele.
                3. Kayışlar, hortumlar, egzoz ve yakıt kaçaklarını kontrol et.
                4. Jeneratörü yük altında çalıştır ve gerilim değerlerini ölç.
                5. Otomatik devreye girme ve transfer panosu testini yap.
                6. Çalışma saatlerini, ölçümleri ve bakım ihtiyaçlarını kaydet.',
                        120,
                        'İş ayakkabısı, eldiven, gözlük, kulaklık',
                        'Sıcak Çalışma İzni',
                        'Motor yağı, yağ filtresi, hava filtresi, akü suyu'
                    ),
                    (
                        'HVAC Mevsimsel Bakım',
                        'Isıtma, havalandırma ve iklimlendirme sistemlerinin bakım planı.',
                        'HVAC',
                        '1. Filtreleri, serpantinleri ve fanları kontrol edip temizle.
                2. Soğutucu akışkan devresinde kaçak ve basınç kontrolü yap.
                3. Kompresör, motor, kayış ve rulmanları incele.
                4. Termostat, sensör ve otomasyon sinyallerini test et.
                5. Drenaj hatlarını temizle ve yoğuşma tahliyesini doğrula.
                6. Sıcaklık, basınç ve enerji değerlerini kaydet; uygunsuzlukları bildir.',
                        150,
                        'İş ayakkabısı, eldiven, gözlük, kulaklık',
                        'Sıcak Çalışma İzni',
                        'Hava filtresi, temizlik kimyasalı, kayış, izolasyon bandı'
                    )
                )
                INSERT INTO "JobPlanTemplates" (
                    "Id",
                    "Name",
                    "Description",
                    "AssetType",
                    "Steps",
                    "EstimatedDurationMinutes",
                    "RequiredPpe",
                    "RequiredPermitType",
                    "RecommendedParts",
                    "TenantId",
                    "CreatedAt",
                    "UpdatedAt",
                    "IsDeleted"
                )
                SELECT
                    md5(t."TenantId"::text || ':turkey-job-plan:' || p."Name")::uuid,
                    p."Name",
                    p."Description",
                    p."AssetType",
                    p."Steps",
                    p."EstimatedDurationMinutes",
                    p."RequiredPpe",
                    p."RequiredPermitType",
                    p."RecommendedParts",
                    t."TenantId",
                    CURRENT_TIMESTAMP,
                    CURRENT_TIMESTAMP,
                    FALSE
                FROM tenants t
                CROSS JOIN templates p
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "JobPlanTemplates" existing
                    WHERE existing."TenantId" = t."TenantId"
                      AND existing."Name" = p."Name"
                      AND NOT existing."IsDeleted"
                );
                """);

            migrationBuilder.Sql(@"ALTER TABLE ""CalibrationRecords"" ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""CalibrationRecords"" FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"CREATE POLICY ""tenant_isolation"" ON ""CalibrationRecords"" USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""tenant_isolation"" ON ""CalibrationRecords"";");
            migrationBuilder.Sql(@"ALTER TABLE ""CalibrationRecords"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""CalibrationRecords"" NO FORCE ROW LEVEL SECURITY;");
            migrationBuilder.DropTable(
                name: "CalibrationRecords");
        }
    }
}
