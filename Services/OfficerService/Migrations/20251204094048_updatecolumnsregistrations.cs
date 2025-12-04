using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updatecolumnsregistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TP_Registration",
                table: "Source_Lat_Long",
                newName: "Registration_No");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Registration_No",
                table: "Source_Lat_Long",
                newName: "TP_Registration");
        }
    }
}
