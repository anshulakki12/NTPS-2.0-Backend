using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterRolesLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Master_Roles_Logs",
                columns: table => new
                {
                    Log_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_Id = table.Column<int>(type: "int", nullable: false),
                    Role_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Operation_Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Operation_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Operation_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Old_Role_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    New_Role_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Old_Is_Active = table.Column<bool>(type: "bit", nullable: true),
                    New_Is_Active = table.Column<bool>(type: "bit", nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Deactivated_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reactivated_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IP_Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Roles_Logs", x => x.Log_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Master_Roles_Logs");
        }
    }
}
