using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updatetpmultiplesttus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Application_Status",
                table: "Application_Master",
                newName: "ApplicationStatus");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationStatus",
                table: "Application_Master",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationStatusId",
                table: "Application_Master",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationStatusId",
                table: "Application_Master");

            migrationBuilder.RenameColumn(
                name: "ApplicationStatus",
                table: "Application_Master",
                newName: "Application_Status");

            migrationBuilder.AlterColumn<string>(
                name: "Application_Status",
                table: "Application_Master",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
