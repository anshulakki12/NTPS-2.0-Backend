using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updatecolumnsregistrationnos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tp_source_place",
                table: "Tp_source_place");

            migrationBuilder.RenameTable(
                name: "Tp_source_place",
                newName: "source_place");

            migrationBuilder.RenameColumn(
                name: "TP_registrationno",
                table: "Tp_Destination_Place",
                newName: "Application_Id");

            migrationBuilder.RenameColumn(
                name: "TP_Registration",
                table: "Species_Logs_SawnTimber",
                newName: "Registration_No");

            migrationBuilder.RenameColumn(
                name: "TP_Registration",
                table: "Species_Logs_RoundTimber",
                newName: "Registration_No");

            migrationBuilder.RenameColumn(
                name: "TP_Registration",
                table: "Species_Logs_MinorForestProduce",
                newName: "Registration_No");

            migrationBuilder.RenameColumn(
                name: "TP_Registration",
                table: "Species_Logs_Fuelwood",
                newName: "Registration_No");

            migrationBuilder.RenameColumn(
                name: "TP_Registration",
                table: "Species_Logs_Bamboo",
                newName: "Registration_No");

            migrationBuilder.RenameColumn(
                name: "Noc_registrationno",
                table: "Noc_source_place",
                newName: "Application_Id");

            migrationBuilder.RenameColumn(
                name: "Noc_registrationno",
                table: "Noc_Destination_Place",
                newName: "Application_Id");

            migrationBuilder.RenameColumn(
                name: "TP_Registration",
                table: "Government_Depot",
                newName: "Registration_No");

            migrationBuilder.RenameColumn(
                name: "TP_registrationno",
                table: "source_place",
                newName: "Application_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_source_place",
                table: "source_place",
                column: "Source_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_source_place",
                table: "source_place");

            migrationBuilder.RenameTable(
                name: "source_place",
                newName: "Tp_source_place");

            migrationBuilder.RenameColumn(
                name: "Application_Id",
                table: "Tp_Destination_Place",
                newName: "TP_registrationno");

            migrationBuilder.RenameColumn(
                name: "Registration_No",
                table: "Species_Logs_SawnTimber",
                newName: "TP_Registration");

            migrationBuilder.RenameColumn(
                name: "Registration_No",
                table: "Species_Logs_RoundTimber",
                newName: "TP_Registration");

            migrationBuilder.RenameColumn(
                name: "Registration_No",
                table: "Species_Logs_MinorForestProduce",
                newName: "TP_Registration");

            migrationBuilder.RenameColumn(
                name: "Registration_No",
                table: "Species_Logs_Fuelwood",
                newName: "TP_Registration");

            migrationBuilder.RenameColumn(
                name: "Registration_No",
                table: "Species_Logs_Bamboo",
                newName: "TP_Registration");

            migrationBuilder.RenameColumn(
                name: "Application_Id",
                table: "Noc_source_place",
                newName: "Noc_registrationno");

            migrationBuilder.RenameColumn(
                name: "Application_Id",
                table: "Noc_Destination_Place",
                newName: "Noc_registrationno");

            migrationBuilder.RenameColumn(
                name: "Registration_No",
                table: "Government_Depot",
                newName: "TP_Registration");

            migrationBuilder.RenameColumn(
                name: "Application_Id",
                table: "Tp_source_place",
                newName: "TP_registrationno");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tp_source_place",
                table: "Tp_source_place",
                column: "Source_Id");
        }
    }
}
