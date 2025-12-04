using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updatespeciecolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Application_Id",
                table: "Species_Logs_SawnTimber",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Application_Id",
                table: "Species_Logs_RoundTimber",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Application_Id",
                table: "Species_Logs_MinorForestProduce",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Application_Id",
                table: "Species_Logs_Fuelwood",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Application_Id",
                table: "Species_Logs_Bamboo",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Application_Id",
                table: "Species_Logs_SawnTimber");

            migrationBuilder.DropColumn(
                name: "Application_Id",
                table: "Species_Logs_RoundTimber");

            migrationBuilder.DropColumn(
                name: "Application_Id",
                table: "Species_Logs_MinorForestProduce");

            migrationBuilder.DropColumn(
                name: "Application_Id",
                table: "Species_Logs_Fuelwood");

            migrationBuilder.DropColumn(
                name: "Application_Id",
                table: "Species_Logs_Bamboo");
        }
    }
}
