using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class createapplicationtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application_Master",
                columns: table => new
                {
                    Application_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    Createdby_UserID = table.Column<int>(type: "int", nullable: true),
                    Createdby_UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    State_ID = table.Column<int>(type: "int", nullable: true),
                    District_ID = table.Column<int>(type: "int", nullable: true),
                    SubDistrict_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application_Master", x => x.Application_ID);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationCategory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationCategory", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Application_Details",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Application_ID = table.Column<long>(type: "bigint", nullable: true),
                    Registration_No = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Application_Type = table.Column<int>(type: "int", nullable: true),
                    CreatedBy_UserID = table.Column<int>(type: "int", nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application_Details", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Application_Details_ApplicationCategory_Application_Type",
                        column: x => x.Application_Type,
                        principalTable: "ApplicationCategory",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Application_Details_Application_Master_Application_ID",
                        column: x => x.Application_ID,
                        principalTable: "Application_Master",
                        principalColumn: "Application_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_Details_Application_ID",
                table: "Application_Details",
                column: "Application_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Details_Application_Type",
                table: "Application_Details",
                column: "Application_Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application_Details");

            migrationBuilder.DropTable(
                name: "ApplicationCategory");

            migrationBuilder.DropTable(
                name: "Application_Master");
        }
    }
}
