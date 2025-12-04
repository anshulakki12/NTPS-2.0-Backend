using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class addupdatesourceproducetables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Noc_Destination_Place",
                columns: table => new
                {
                    Destination_Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Noc_registrationno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State_Id = table.Column<int>(type: "int", nullable: false),
                    Circle_Id = table.Column<int>(type: "int", nullable: false),
                    Division_Id = table.Column<int>(type: "int", nullable: false),
                    Range_Id = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Pincode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Noc_Destination_Place", x => x.Destination_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Noc_Destination_Place");
        }
    }
}
