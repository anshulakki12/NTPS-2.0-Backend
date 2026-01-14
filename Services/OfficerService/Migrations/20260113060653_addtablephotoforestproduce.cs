using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class addtablephotoforestproduce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Photo_Forest_Produce",
                columns: table => new
                {
                    Photo_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Registration_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Photo_Upload = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Source_type = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photo_Forest_Produce", x => x.Photo_ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photo_Forest_Produce");
        }
    }
}
