using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeciesService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Forest_Produce");

            migrationBuilder.DropTable(
                name: "Master_Species");
        }
    }
}
