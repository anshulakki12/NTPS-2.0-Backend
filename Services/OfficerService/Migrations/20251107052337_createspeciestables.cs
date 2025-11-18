using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class createspeciestables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Master_Zone_State_State_ID",
                table: "Master_Zone");

            migrationBuilder.CreateTable(
                name: "Forest_Produce",
                columns: table => new
                {
                    Forest_Produce_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Quantity_Type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Weight_Type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Category = table.Column<int>(type: "int", nullable: true),
                    Requires_Log = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forest_Produce", x => x.Forest_Produce_Id);
                });

            migrationBuilder.CreateTable(
                name: "Master_Species",
                columns: table => new
                {
                    SpeciesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Species", x => x.SpeciesID);
                });

            migrationBuilder.CreateTable(
                name: "Species_Mapping",
                columns: table => new
                {
                    Species_Mapping_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Forest_Produce_Id = table.Column<int>(type: "int", nullable: false),
                    Species_ID = table.Column<int>(type: "int", nullable: false),
                    State_ID = table.Column<int>(type: "int", nullable: false),
                    WorkFlow_ID = table.Column<int>(type: "int", nullable: false),
                    Zone_ID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species_Mapping", x => x.Species_Mapping_ID);
                    table.ForeignKey(
                        name: "FK_Species_Mapping_Forest_Produce_Forest_Produce_Id",
                        column: x => x.Forest_Produce_Id,
                        principalTable: "Forest_Produce",
                        principalColumn: "Forest_Produce_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Species_Mapping_Master_Species_Species_ID",
                        column: x => x.Species_ID,
                        principalTable: "Master_Species",
                        principalColumn: "SpeciesID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Species_Mapping_Master_WorkFlow_WorkFlow_ID",
                        column: x => x.WorkFlow_ID,
                        principalTable: "Master_WorkFlow",
                        principalColumn: "WorkFlow_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Species_Mapping_Master_Zone_Zone_ID",
                        column: x => x.Zone_ID,
                        principalTable: "Master_Zone",
                        principalColumn: "Zone_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Species_Mapping_State_State_ID",
                        column: x => x.State_ID,
                        principalSchema: "dbo",
                        principalTable: "State",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Species_Mapping_Forest_Produce_Id",
                table: "Species_Mapping",
                column: "Forest_Produce_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Mapping_Species_ID",
                table: "Species_Mapping",
                column: "Species_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Mapping_State_ID",
                table: "Species_Mapping",
                column: "State_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Mapping_WorkFlow_ID",
                table: "Species_Mapping",
                column: "WorkFlow_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Mapping_Zone_ID",
                table: "Species_Mapping",
                column: "Zone_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Master_Zone_State_State_ID",
                table: "Master_Zone",
                column: "State_ID",
                principalSchema: "dbo",
                principalTable: "State",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Master_Zone_State_State_ID",
                table: "Master_Zone");

            migrationBuilder.DropTable(
                name: "Species_Mapping");

            migrationBuilder.DropTable(
                name: "Forest_Produce");

            migrationBuilder.DropTable(
                name: "Master_Species");

            migrationBuilder.AddForeignKey(
                name: "FK_Master_Zone_State_State_ID",
                table: "Master_Zone",
                column: "State_ID",
                principalSchema: "dbo",
                principalTable: "State",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
