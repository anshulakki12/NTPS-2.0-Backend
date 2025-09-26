using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeciesService.Migrations
{
    /// <inheritdoc />
    public partial class addspeciesexempted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Species_Exempted",
                columns: table => new
                {
                    Exempted_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State_Id = table.Column<int>(type: "int", nullable: true),
                    DIST_CODE = table.Column<int>(type: "int", nullable: true),
                    Species_Id = table.Column<int>(type: "int", nullable: true),
                    ProduceSpeciesID = table.Column<int>(type: "int", nullable: true),
                    ExemptORNotExempt = table.Column<int>(type: "int", nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species_Exempted", x => x.Exempted_Id);
                    table.ForeignKey(
                        name: "FK_Species_Exempted_Forest_Produce_ProduceSpeciesID",
                        column: x => x.ProduceSpeciesID,
                        principalTable: "Forest_Produce",
                        principalColumn: "Forest_Produce_Id");
                    table.ForeignKey(
                        name: "FK_Species_Exempted_Master_Species_Species_Id",
                        column: x => x.Species_Id,
                        principalTable: "Master_Species",
                        principalColumn: "SpeciesID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Species_Exempted_ProduceSpeciesID",
                table: "Species_Exempted",
                column: "ProduceSpeciesID");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Exempted_Species_Id",
                table: "Species_Exempted",
                column: "Species_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Species_Exempted");
        }
    }
}
