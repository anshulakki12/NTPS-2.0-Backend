using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationService.Migrations
{
    /// <inheritdoc />
    public partial class columnsupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginSource",
                table: "Master_Registration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserRole",
                table: "Master_Registration",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginSource",
                table: "Master_Registration");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "Master_Registration");
        }
    }
}
