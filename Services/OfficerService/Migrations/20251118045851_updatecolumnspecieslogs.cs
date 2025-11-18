using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updatecolumnspecieslogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Species_Logs_Bamboo",
                columns: table => new
                {
                    Bamboo_Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TP_Registration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Species_ID = table.Column<int>(type: "int", nullable: false),
                    Girth_Class = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species_Logs_Bamboo", x => x.Bamboo_Id);
                });

            migrationBuilder.CreateTable(
                name: "Species_Logs_Fuelwood",
                columns: table => new
                {
                    Fuelwood_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TP_Registration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Species_ID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species_Logs_Fuelwood", x => x.Fuelwood_ID);
                });

            migrationBuilder.CreateTable(
                name: "Species_Logs_MinorForestProduce",
                columns: table => new
                {
                    MinorForestProduce_Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TP_Registration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Species_ID = table.Column<int>(type: "int", nullable: false),
                    PlantPartID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species_Logs_MinorForestProduce", x => x.MinorForestProduce_Id);
                });

            migrationBuilder.CreateTable(
                name: "Species_Logs_RoundTimber",
                columns: table => new
                {
                    RoundTimber_Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TP_Registration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Species_ID = table.Column<int>(type: "int", nullable: false),
                    Logs_No = table.Column<int>(type: "int", nullable: false),
                    Girth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species_Logs_RoundTimber", x => x.RoundTimber_Id);
                });

            migrationBuilder.CreateTable(
                name: "Species_Logs_SawnTimber",
                columns: table => new
                {
                    SawnTimber_Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TP_Registration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Species_ID = table.Column<int>(type: "int", nullable: false),
                    LogsNo = table.Column<int>(type: "int", nullable: false),
                    Girth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Thickness = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species_Logs_SawnTimber", x => x.SawnTimber_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Species_Logs_Bamboo");

            migrationBuilder.DropTable(
                name: "Species_Logs_Fuelwood");

            migrationBuilder.DropTable(
                name: "Species_Logs_MinorForestProduce");

            migrationBuilder.DropTable(
                name: "Species_Logs_RoundTimber");

            migrationBuilder.DropTable(
                name: "Species_Logs_SawnTimber");
        }
    }
}
