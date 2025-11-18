using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Master_Roles",
                columns: table => new
                {
                    Role_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_By = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Roles", x => x.Role_Id);
                });

            migrationBuilder.CreateTable(
                name: "Officer_Details",
                columns: table => new
                {
                    Officer_Detail_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Officer_Login_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Officer_Title = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Officer_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Officer_Designation_Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Officer_Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Officer_Email_Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Officer_Details", x => x.Officer_Detail_Id);
                });

            migrationBuilder.CreateTable(
                name: "Officer_Registration",
                columns: table => new
                {
                    Officer_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login_Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mobile_No = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_id = table.Column<int>(type: "int", nullable: false),
                    Location_Type = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_By = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Officer_Registration", x => x.Officer_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Master_Roles");

            migrationBuilder.DropTable(
                name: "Officer_Details");

            migrationBuilder.DropTable(
                name: "Officer_Registration");
        }
    }
}
