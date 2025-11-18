using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class Initials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "DISTRICT",
                columns: table => new
                {
                    DIST_CODE = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DIST_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ST_CODE = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRICT", x => x.DIST_CODE);
                });

            migrationBuilder.CreateTable(
                name: "State",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ST_CODE = table.Column<int>(type: "int", nullable: false),
                    ST_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ST_UT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Sub_District",
                columns: table => new
                {
                    Sub_Dist_Code = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sub_Dist_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Dist_Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_District", x => x.Sub_Dist_Code);
                    table.ForeignKey(
                        name: "FK_Sub_District_DISTRICT_Dist_Code",
                        column: x => x.Dist_Code,
                        principalTable: "DISTRICT",
                        principalColumn: "DIST_CODE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Master_Zone",
                columns: table => new
                {
                    Zone_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Zone_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State_ID = table.Column<int>(type: "int", nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_By = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Zone", x => x.Zone_Id);
                    table.ForeignKey(
                        name: "FK_Master_Zone_State_State_ID",
                        column: x => x.State_ID,
                        principalSchema: "dbo",
                        principalTable: "State",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZoneData",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateID = table.Column<int>(type: "int", nullable: false),
                    DistrictID = table.Column<int>(type: "int", nullable: false),
                    SubDistrictID = table.Column<int>(type: "int", nullable: true),
                    ZoneID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ZoneData_DISTRICT_DistrictID",
                        column: x => x.DistrictID,
                        principalTable: "DISTRICT",
                        principalColumn: "DIST_CODE",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZoneData_Master_Zone_ZoneID",
                        column: x => x.ZoneID,
                        principalTable: "Master_Zone",
                        principalColumn: "Zone_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZoneData_State_StateID",
                        column: x => x.StateID,
                        principalSchema: "dbo",
                        principalTable: "State",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZoneData_Sub_District_SubDistrictID",
                        column: x => x.SubDistrictID,
                        principalTable: "Sub_District",
                        principalColumn: "Sub_Dist_Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Master_Zone_State_ID",
                table: "Master_Zone",
                column: "State_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_District_Dist_Code",
                table: "Sub_District",
                column: "Dist_Code");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneData_DistrictID",
                table: "ZoneData",
                column: "DistrictID");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneData_StateID",
                table: "ZoneData",
                column: "StateID");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneData_SubDistrictID",
                table: "ZoneData",
                column: "SubDistrictID");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneData_ZoneID",
                table: "ZoneData",
                column: "ZoneID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ZoneData");

            migrationBuilder.DropTable(
                name: "Master_Zone");

            migrationBuilder.DropTable(
                name: "Sub_District");

            migrationBuilder.DropTable(
                name: "State",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DISTRICT");
        }
    }
}
