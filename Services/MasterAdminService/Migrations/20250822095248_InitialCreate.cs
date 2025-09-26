using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterAdminService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    State_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Region_Id = table.Column<int>(type: "int", nullable: true),
                    State_Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StatURL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TP_Validity = table.Column<int>(type: "int", nullable: true),
                    Required_Field = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Language_Id = table.Column<int>(type: "int", nullable: true),
                    ST_CODE = table.Column<int>(type: "int", nullable: true),
                    Species_DOC = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Local_Rule = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Show = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    Lock = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.State_Id);
                });

            migrationBuilder.CreateTable(
                name: "Circle",
                columns: table => new
                {
                    Circle_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State_Id = table.Column<int>(type: "int", nullable: true),
                    Circle_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TP_Validity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Circle", x => x.Circle_Id);
                    table.ForeignKey(
                        name: "FK_Circle_State_State_Id",
                        column: x => x.State_Id,
                        principalTable: "State",
                        principalColumn: "State_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    Division_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Circle_Id = table.Column<int>(type: "int", nullable: true),
                    Division_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DIST_CODE = table.Column<int>(type: "int", nullable: true),
                    TP_Validity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.Division_Id);
                    table.ForeignKey(
                        name: "FK_Division_Circle_Circle_Id",
                        column: x => x.Circle_Id,
                        principalTable: "Circle",
                        principalColumn: "Circle_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sub_Division",
                columns: table => new
                {
                    Sub_Div_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Division_Id = table.Column<int>(type: "int", nullable: true),
                    Sub_Div_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DivisionId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_Division", x => x.Sub_Div_Id);
                    table.ForeignKey(
                        name: "FK_Sub_Division_Division_DivisionId1",
                        column: x => x.DivisionId1,
                        principalTable: "Division",
                        principalColumn: "Division_Id");
                    table.ForeignKey(
                        name: "FK_Sub_Division_Division_Division_Id",
                        column: x => x.Division_Id,
                        principalTable: "Division",
                        principalColumn: "Division_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Range",
                columns: table => new
                {
                    Range_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Division_Id = table.Column<int>(type: "int", nullable: true),
                    Sub_Div_Id = table.Column<int>(type: "int", nullable: true),
                    Range_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Uploaded_Hammer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TP_Validity = table.Column<int>(type: "int", nullable: false),
                    DivisionId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Range", x => x.Range_Id);
                    table.ForeignKey(
                        name: "FK_Range_Division_DivisionId1",
                        column: x => x.DivisionId1,
                        principalTable: "Division",
                        principalColumn: "Division_Id");
                    table.ForeignKey(
                        name: "FK_Range_Division_Division_Id",
                        column: x => x.Division_Id,
                        principalTable: "Division",
                        principalColumn: "Division_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Range_Sub_Division_Sub_Div_Id",
                        column: x => x.Sub_Div_Id,
                        principalTable: "Sub_Division",
                        principalColumn: "Sub_Div_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Circle_State_Id",
                table: "Circle",
                column: "State_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Division_Circle_Id",
                table: "Division",
                column: "Circle_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Range_Division_Id",
                table: "Range",
                column: "Division_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Range_DivisionId1",
                table: "Range",
                column: "DivisionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Range_Sub_Div_Id",
                table: "Range",
                column: "Sub_Div_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Division_Division_Id",
                table: "Sub_Division",
                column: "Division_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Division_DivisionId1",
                table: "Sub_Division",
                column: "DivisionId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Range");

            migrationBuilder.DropTable(
                name: "Sub_Division");

            migrationBuilder.DropTable(
                name: "Division");

            migrationBuilder.DropTable(
                name: "Circle");

            migrationBuilder.DropTable(
                name: "State");
        }
    }
}
