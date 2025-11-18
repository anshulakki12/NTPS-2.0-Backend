using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationService.Migrations
{
    /// <inheritdoc />
    public partial class tablenameupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Master_Registration");

            migrationBuilder.CreateTable(
                name: "Applicant_Registration",
                columns: table => new
                {
                    Registration_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name_Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Login_Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile_No = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Is_Verified = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Registration_Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserRole = table.Column<int>(type: "int", nullable: true),
                    LoginSource = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicant_Registration", x => x.Registration_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applicant_Registration");

            migrationBuilder.CreateTable(
                name: "Master_Registration",
                columns: table => new
                {
                    Registration_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Is_Verified = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    Login_Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginSource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile_No = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name_Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Registration_Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserRole = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Registration", x => x.Registration_id);
                });
        }
    }
}
