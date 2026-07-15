using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingehOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalTwinForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FloorPlans_FacilityId",
                table: "FloorPlans",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPositions_AssetId",
                table: "AssetPositions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPositions_FloorPlanId",
                table: "AssetPositions",
                column: "FloorPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetPositions_Assets_AssetId",
                table: "AssetPositions",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetPositions_FloorPlans_FloorPlanId",
                table: "AssetPositions",
                column: "FloorPlanId",
                principalTable: "FloorPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FloorPlans_Facilities_FacilityId",
                table: "FloorPlans",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetPositions_Assets_AssetId",
                table: "AssetPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetPositions_FloorPlans_FloorPlanId",
                table: "AssetPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_FloorPlans_Facilities_FacilityId",
                table: "FloorPlans");

            migrationBuilder.DropIndex(
                name: "IX_FloorPlans_FacilityId",
                table: "FloorPlans");

            migrationBuilder.DropIndex(
                name: "IX_AssetPositions_AssetId",
                table: "AssetPositions");

            migrationBuilder.DropIndex(
                name: "IX_AssetPositions_FloorPlanId",
                table: "AssetPositions");
        }
    }
}
