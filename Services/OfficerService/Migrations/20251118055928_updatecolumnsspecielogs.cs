using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updatecolumnsspecielogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Application_Master",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy_UserId",
                table: "Application_Master",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy_UserName",
                table: "Application_Master",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated_Date",
                table: "Application_Master",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_Logs_SawnTimber_Species_ID",
                table: "Species_Logs_SawnTimber",
                column: "Species_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Logs_RoundTimber_Species_ID",
                table: "Species_Logs_RoundTimber",
                column: "Species_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Logs_MinorForestProduce_Species_ID",
                table: "Species_Logs_MinorForestProduce",
                column: "Species_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Logs_Fuelwood_Species_ID",
                table: "Species_Logs_Fuelwood",
                column: "Species_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Logs_Bamboo_Species_ID",
                table: "Species_Logs_Bamboo",
                column: "Species_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Logs_Bamboo_Master_Species_Species_ID",
                table: "Species_Logs_Bamboo",
                column: "Species_ID",
                principalTable: "Master_Species",
                principalColumn: "SpeciesID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Logs_Fuelwood_Master_Species_Species_ID",
                table: "Species_Logs_Fuelwood",
                column: "Species_ID",
                principalTable: "Master_Species",
                principalColumn: "SpeciesID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Logs_MinorForestProduce_Master_Species_Species_ID",
                table: "Species_Logs_MinorForestProduce",
                column: "Species_ID",
                principalTable: "Master_Species",
                principalColumn: "SpeciesID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Logs_RoundTimber_Master_Species_Species_ID",
                table: "Species_Logs_RoundTimber",
                column: "Species_ID",
                principalTable: "Master_Species",
                principalColumn: "SpeciesID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Logs_SawnTimber_Master_Species_Species_ID",
                table: "Species_Logs_SawnTimber",
                column: "Species_ID",
                principalTable: "Master_Species",
                principalColumn: "SpeciesID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Species_Logs_Bamboo_Master_Species_Species_ID",
                table: "Species_Logs_Bamboo");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_Logs_Fuelwood_Master_Species_Species_ID",
                table: "Species_Logs_Fuelwood");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_Logs_MinorForestProduce_Master_Species_Species_ID",
                table: "Species_Logs_MinorForestProduce");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_Logs_RoundTimber_Master_Species_Species_ID",
                table: "Species_Logs_RoundTimber");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_Logs_SawnTimber_Master_Species_Species_ID",
                table: "Species_Logs_SawnTimber");

            migrationBuilder.DropIndex(
                name: "IX_Species_Logs_SawnTimber_Species_ID",
                table: "Species_Logs_SawnTimber");

            migrationBuilder.DropIndex(
                name: "IX_Species_Logs_RoundTimber_Species_ID",
                table: "Species_Logs_RoundTimber");

            migrationBuilder.DropIndex(
                name: "IX_Species_Logs_MinorForestProduce_Species_ID",
                table: "Species_Logs_MinorForestProduce");

            migrationBuilder.DropIndex(
                name: "IX_Species_Logs_Fuelwood_Species_ID",
                table: "Species_Logs_Fuelwood");

            migrationBuilder.DropIndex(
                name: "IX_Species_Logs_Bamboo_Species_ID",
                table: "Species_Logs_Bamboo");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Application_Master");

            migrationBuilder.DropColumn(
                name: "UpdatedBy_UserId",
                table: "Application_Master");

            migrationBuilder.DropColumn(
                name: "UpdatedBy_UserName",
                table: "Application_Master");

            migrationBuilder.DropColumn(
                name: "Updated_Date",
                table: "Application_Master");
        }
    }
}
