using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updatespecieslogstablesmappingid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Species_Mapping_ID",
                table: "Species_Logs_SawnTimber",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Species_Mapping_ID",
                table: "Species_Logs_RoundTimber",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Species_Mapping_ID",
                table: "Species_Logs_MinorForestProduce",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Species_Mapping_ID",
                table: "Species_Logs_Fuelwood",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Species_Mapping_ID",
                table: "Species_Logs_Bamboo",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Species_Mapping_ID",
                table: "Species_Logs_SawnTimber");

            migrationBuilder.DropColumn(
                name: "Species_Mapping_ID",
                table: "Species_Logs_RoundTimber");

            migrationBuilder.DropColumn(
                name: "Species_Mapping_ID",
                table: "Species_Logs_MinorForestProduce");

            migrationBuilder.DropColumn(
                name: "Species_Mapping_ID",
                table: "Species_Logs_Fuelwood");

            migrationBuilder.DropColumn(
                name: "Species_Mapping_ID",
                table: "Species_Logs_Bamboo");
        }
    }
}
