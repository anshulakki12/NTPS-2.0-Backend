using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicMenuSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Master_Menus",
                columns: table => new
                {
                    Menu_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Menu_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Menu_Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Module_Id = table.Column<int>(type: "int", nullable: false),
                    Parent_Menu_Id = table.Column<int>(type: "int", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Router_Link = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Target = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Is_Parent = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_By = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_By = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Menus", x => x.Menu_Id);
                    table.ForeignKey(
                        name: "FK_Master_Menus_Master_Menus_Parent_Menu_Id",
                        column: x => x.Parent_Menu_Id,
                        principalTable: "Master_Menus",
                        principalColumn: "Menu_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Master_Menus_Master_Modules_Module_Id",
                        column: x => x.Module_Id,
                        principalTable: "Master_Modules",
                        principalColumn: "Module_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Role_Menus",
                columns: table => new
                {
                    Role_Menu_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_Id = table.Column<int>(type: "int", nullable: false),
                    Menu_Id = table.Column<int>(type: "int", nullable: false),
                    Display_Order = table.Column<int>(type: "int", nullable: false),
                    Is_Visible = table.Column<bool>(type: "bit", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_By = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_By = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_Menus", x => x.Role_Menu_Id);
                    table.ForeignKey(
                        name: "FK_Role_Menus_Master_Menus_Menu_Id",
                        column: x => x.Menu_Id,
                        principalTable: "Master_Menus",
                        principalColumn: "Menu_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Role_Menus_Master_Roles_Role_Id",
                        column: x => x.Role_Id,
                        principalTable: "Master_Roles",
                        principalColumn: "Role_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Master_Menus_Menu_Code",
                table: "Master_Menus",
                column: "Menu_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Master_Menus_Module_Id",
                table: "Master_Menus",
                column: "Module_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Master_Menus_Parent_Menu_Id",
                table: "Master_Menus",
                column: "Parent_Menu_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Menus_Menu_Id",
                table: "Role_Menus",
                column: "Menu_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Menus_Role_Id_Menu_Id",
                table: "Role_Menus",
                columns: new[] { "Role_Id", "Menu_Id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Role_Menus");

            migrationBuilder.DropTable(
                name: "Master_Menus");
        }
    }
}
