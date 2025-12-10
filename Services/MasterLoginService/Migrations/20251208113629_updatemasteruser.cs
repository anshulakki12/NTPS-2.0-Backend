using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterLoginService.Migrations
{
    /// <inheritdoc />
    public partial class updatemasteruser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created_Date",
                table: "Master_User",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Is_Active",
                table: "Master_User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Is_Verified",
                table: "Master_User",
                type: "char(1)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Last_Updated",
                table: "Master_User",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Master_User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Registration_Type",
                table: "Master_User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created_Date",
                table: "Master_User");

            migrationBuilder.DropColumn(
                name: "Is_Active",
                table: "Master_User");

            migrationBuilder.DropColumn(
                name: "Is_Verified",
                table: "Master_User");

            migrationBuilder.DropColumn(
                name: "Last_Updated",
                table: "Master_User");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Master_User");

            migrationBuilder.DropColumn(
                name: "Registration_Type",
                table: "Master_User");
        }
    }
}
