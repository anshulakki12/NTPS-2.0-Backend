using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class addtablesapplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Government_Depot",
                columns: table => new
                {
                    Government_Depot_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TP_Registration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Depot_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Log_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date_of_Auction = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount_Paid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Bill_Upload = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gov_Depot_Id = table.Column<int>(type: "int", nullable: true),
                    source_Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Place = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Government_Depot", x => x.Government_Depot_Id);
                });

            migrationBuilder.CreateTable(
                name: "Source_Lat_Long",
                columns: table => new
                {
                    SourceLatLong_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TP_Registration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Latitude = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Longitude = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source_Lat_Long", x => x.SourceLatLong_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tp_Destination_Place",
                columns: table => new
                {
                    Destination_Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TP_registrationno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_Tp_Destination_Place", x => x.Destination_Id);
                });

            migrationBuilder.CreateTable(
                name: "Tp_source_place",
                columns: table => new
                {
                    Source_Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TP_registrationno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_Tp_source_place", x => x.Source_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Government_Depot");

            migrationBuilder.DropTable(
                name: "Source_Lat_Long");

            migrationBuilder.DropTable(
                name: "Tp_Destination_Place");

            migrationBuilder.DropTable(
                name: "Tp_source_place");
        }
    }
}
