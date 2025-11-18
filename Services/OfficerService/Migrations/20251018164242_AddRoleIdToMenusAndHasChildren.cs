using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleIdToMenusAndHasChildren : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Has_Children",
                table: "Master_Menus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // First add Role_Id as nullable
            migrationBuilder.AddColumn<int>(
                name: "Role_Id",
                table: "Master_Menus",
                type: "int",
                nullable: true);

            // Update existing menus to have a default role (assuming Role_Id = 1 exists - Admin role)
            // You can change this to match your specific role structure
            migrationBuilder.Sql(
                @"UPDATE Master_Menus 
                  SET Role_Id = 1 
                  WHERE Role_Id IS NULL");

            // Now alter the column to be NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "Role_Id",
                table: "Master_Menus",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Master_Menus_Role_Id",
                table: "Master_Menus",
                column: "Role_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Master_Menus_Master_Roles_Role_Id",
                table: "Master_Menus",
                column: "Role_Id",
                principalTable: "Master_Roles",
                principalColumn: "Role_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Master_Menus_Master_Roles_Role_Id",
                table: "Master_Menus");

            migrationBuilder.DropIndex(
                name: "IX_Master_Menus_Role_Id",
                table: "Master_Menus");

            migrationBuilder.DropColumn(
                name: "Has_Children",
                table: "Master_Menus");

            migrationBuilder.DropColumn(
                name: "Role_Id",
                table: "Master_Menus");
        }
    }
}
