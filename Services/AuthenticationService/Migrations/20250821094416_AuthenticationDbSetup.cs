using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationService.Migrations
{
    /// <inheritdoc />
    public partial class AuthenticationDbSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applicant_Personal_Details",
                columns: table => new
                {
                    Details_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ID_Proof = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ID_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ID_Upload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State_Id = table.Column<int>(type: "int", nullable: true),
                    Circle_id = table.Column<int>(type: "int", nullable: true),
                    Division_id = table.Column<int>(type: "int", nullable: true),
                    Sub_division_id = table.Column<int>(type: "int", nullable: true),
                    Range_Id = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pin_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modification_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source_Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicant_Personal_Details", x => x.Details_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applicant_Personal_Details");
        }
    }
}
