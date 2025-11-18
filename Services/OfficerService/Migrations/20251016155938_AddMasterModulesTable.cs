using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterModulesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Master_Modules",
                columns: table => new
                {
                    Module_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Module_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Module_Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Route_Path = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Display_Order = table.Column<int>(type: "int", nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_By = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Modified_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modified_By = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Deactivated_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reactivated_On = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Modules", x => x.Module_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Master_Modules_Module_Code",
                table: "Master_Modules",
                column: "Module_Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Master_Modules");
        }
    }
}
