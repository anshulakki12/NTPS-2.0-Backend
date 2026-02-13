using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updatemastertransportmodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Created_By",
                table: "Master_Transport_Mode",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created_Date",
                table: "Master_Transport_Mode",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Active",
                table: "Master_Transport_Mode",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Modified_By",
                table: "Master_Transport_Mode",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified_Date",
                table: "Master_Transport_Mode",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created_By",
                table: "Master_Transport_Mode");

            migrationBuilder.DropColumn(
                name: "Created_Date",
                table: "Master_Transport_Mode");

            migrationBuilder.DropColumn(
                name: "Is_Active",
                table: "Master_Transport_Mode");

            migrationBuilder.DropColumn(
                name: "Modified_By",
                table: "Master_Transport_Mode");

            migrationBuilder.DropColumn(
                name: "Modified_Date",
                table: "Master_Transport_Mode");
        }
    }
}
