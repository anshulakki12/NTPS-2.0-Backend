using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterDesignationAndLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "designation_id",
                table: "Officer_Registration",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Master_Designation",
                columns: table => new
                {
                    Designation_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Designation_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Deactivated_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reactivated_On = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Designation", x => x.Designation_Id);
                });

            migrationBuilder.CreateTable(
                name: "Master_Designation_Logs",
                columns: table => new
                {
                    Log_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Designation_Id = table.Column<int>(type: "int", nullable: false),
                    Designation_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: true),
                    Abbreviation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Operation_Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Operation_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Operation_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Old_Designation_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    New_Designation_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Old_Rank = table.Column<int>(type: "int", nullable: true),
                    New_Rank = table.Column<int>(type: "int", nullable: true),
                    Old_Abbreviation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    New_Abbreviation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
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
                    table.PrimaryKey("PK_Master_Designation_Logs", x => x.Log_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Officer_Registration_designation_id",
                table: "Officer_Registration",
                column: "designation_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Officer_Registration_Master_Designation_designation_id",
                table: "Officer_Registration",
                column: "designation_id",
                principalTable: "Master_Designation",
                principalColumn: "Designation_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Officer_Registration_Master_Designation_designation_id",
                table: "Officer_Registration");

            migrationBuilder.DropTable(
                name: "Master_Designation");

            migrationBuilder.DropTable(
                name: "Master_Designation_Logs");

            migrationBuilder.DropIndex(
                name: "IX_Officer_Registration_designation_id",
                table: "Officer_Registration");

            migrationBuilder.DropColumn(
                name: "designation_id",
                table: "Officer_Registration");
        }
    }
}
