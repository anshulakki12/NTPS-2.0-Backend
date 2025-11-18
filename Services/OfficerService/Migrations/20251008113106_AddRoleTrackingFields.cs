using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleTrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Deactivated_On",
                table: "Master_Roles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Reactivated_On",
                table: "Master_Roles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Updated_By",
                table: "Master_Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated_Date",
                table: "Master_Roles",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deactivated_On",
                table: "Master_Roles");

            migrationBuilder.DropColumn(
                name: "Reactivated_On",
                table: "Master_Roles");

            migrationBuilder.DropColumn(
                name: "Updated_By",
                table: "Master_Roles");

            migrationBuilder.DropColumn(
                name: "Updated_Date",
                table: "Master_Roles");
        }
    }
}
