using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updatespecieslogstables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateIndex(
                name: "IX_Species_Logs_SawnTimber_ForestProduceId",
                table: "Species_Logs_SawnTimber",
                column: "ForestProduceId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Logs_RoundTimber_ForestProduceId",
                table: "Species_Logs_RoundTimber",
                column: "ForestProduceId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Logs_MinorForestProduce_ForestProduceId",
                table: "Species_Logs_MinorForestProduce",
                column: "ForestProduceId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Logs_Fuelwood_ForestProduceId",
                table: "Species_Logs_Fuelwood",
                column: "ForestProduceId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Logs_Bamboo_ForestProduceId",
                table: "Species_Logs_Bamboo",
                column: "ForestProduceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Logs_Bamboo_Forest_Produce_ForestProduceId",
                table: "Species_Logs_Bamboo",
                column: "ForestProduceId",
                principalTable: "Forest_Produce",
                principalColumn: "Forest_Produce_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Logs_Fuelwood_Forest_Produce_ForestProduceId",
                table: "Species_Logs_Fuelwood",
                column: "ForestProduceId",
                principalTable: "Forest_Produce",
                principalColumn: "Forest_Produce_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Logs_MinorForestProduce_Forest_Produce_ForestProduceId",
                table: "Species_Logs_MinorForestProduce",
                column: "ForestProduceId",
                principalTable: "Forest_Produce",
                principalColumn: "Forest_Produce_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Logs_RoundTimber_Forest_Produce_ForestProduceId",
                table: "Species_Logs_RoundTimber",
                column: "ForestProduceId",
                principalTable: "Forest_Produce",
                principalColumn: "Forest_Produce_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Logs_SawnTimber_Forest_Produce_ForestProduceId",
                table: "Species_Logs_SawnTimber",
                column: "ForestProduceId",
                principalTable: "Forest_Produce",
                principalColumn: "Forest_Produce_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Species_Logs_Bamboo_Forest_Produce_ForestProduceId",
                table: "Species_Logs_Bamboo");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_Logs_Fuelwood_Forest_Produce_ForestProduceId",
                table: "Species_Logs_Fuelwood");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_Logs_MinorForestProduce_Forest_Produce_ForestProduceId",
                table: "Species_Logs_MinorForestProduce");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_Logs_RoundTimber_Forest_Produce_ForestProduceId",
                table: "Species_Logs_RoundTimber");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_Logs_SawnTimber_Forest_Produce_ForestProduceId",
                table: "Species_Logs_SawnTimber");

            migrationBuilder.DropIndex(
                name: "IX_Species_Logs_SawnTimber_ForestProduceId",
                table: "Species_Logs_SawnTimber");

            migrationBuilder.DropIndex(
                name: "IX_Species_Logs_RoundTimber_ForestProduceId",
                table: "Species_Logs_RoundTimber");

            migrationBuilder.DropIndex(
                name: "IX_Species_Logs_MinorForestProduce_ForestProduceId",
                table: "Species_Logs_MinorForestProduce");

            migrationBuilder.DropIndex(
                name: "IX_Species_Logs_Fuelwood_ForestProduceId",
                table: "Species_Logs_Fuelwood");

            migrationBuilder.DropIndex(
                name: "IX_Species_Logs_Bamboo_ForestProduceId",
                table: "Species_Logs_Bamboo");

            migrationBuilder.DropColumn(
                name: "ForestProduceId",
                table: "Species_Logs_SawnTimber");

            migrationBuilder.DropColumn(
                name: "ForestProduceId",
                table: "Species_Logs_RoundTimber");

            migrationBuilder.DropColumn(
                name: "ForestProduceId",
                table: "Species_Logs_MinorForestProduce");

            migrationBuilder.DropColumn(
                name: "ForestProduceId",
                table: "Species_Logs_Fuelwood");

            migrationBuilder.DropColumn(
                name: "ForestProduceId",
                table: "Species_Logs_Bamboo");
        }
    }
}
