using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class addtbltransportdetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transport_Details",
                columns: table => new
                {
                    TP_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Registration_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Transport_Id = table.Column<int>(type: "int", nullable: false),
                    Driver_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Driver_licence_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Vehicle_Owner_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Vehicle_No = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Vehicle_Photograph = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    source_type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Vehicle_LoadingCertificate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transport_Details", x => x.TP_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transport_Details");
        }
    }
}
