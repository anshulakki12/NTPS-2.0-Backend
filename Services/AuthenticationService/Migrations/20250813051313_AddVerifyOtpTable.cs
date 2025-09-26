using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationService.Migrations
{
    /// <inheritdoc />
    public partial class AddVerifyOtpTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Verify_Otp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mobile_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Application_Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OTP_Cases_Id = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OTP = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    Attempt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verify_Otp", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Verify_Otp");
        }
    }
}
