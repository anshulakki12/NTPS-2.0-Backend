using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class sourceplacenamechange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_source_place",
                table: "source_place");

            migrationBuilder.RenameTable(
                name: "source_place",
                newName: "Tp_source_place");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tp_source_place",
                table: "Tp_source_place",
                column: "Source_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tp_source_place",
                table: "Tp_source_place");

            migrationBuilder.RenameTable(
                name: "Tp_source_place",
                newName: "source_place");

            migrationBuilder.AddPrimaryKey(
                name: "PK_source_place",
                table: "source_place",
                column: "Source_Id");
        }
    }
}
