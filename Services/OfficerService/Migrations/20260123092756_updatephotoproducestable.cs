using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updatephotoproducestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Application_Id",
                table: "Photo_Forest_Produce",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Category_Id",
                table: "Photo_Forest_Produce",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Other_Document",
                table: "Photo_Forest_Produce",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Application_Id",
                table: "Photo_Forest_Produce");

            migrationBuilder.DropColumn(
                name: "Category_Id",
                table: "Photo_Forest_Produce");

            migrationBuilder.DropColumn(
                name: "Other_Document",
                table: "Photo_Forest_Produce");
        }
    }
}
