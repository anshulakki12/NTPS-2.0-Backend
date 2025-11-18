using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class workflowscreatetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Master_Level",
                columns: table => new
                {
                    Level_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Level", x => x.Level_ID);
                });

            migrationBuilder.CreateTable(
                name: "Master_WorkFlow",
                columns: table => new
                {
                    WorkFlow_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkFlow_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StateID = table.Column<int>(type: "int", nullable: false),
                    Is_Default = table.Column<bool>(type: "bit", nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_WorkFlow", x => x.WorkFlow_ID);
                    table.ForeignKey(
                        name: "FK_Master_WorkFlow_State_StateID",
                        column: x => x.StateID,
                        principalSchema: "dbo",
                        principalTable: "State",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowSteps",
                columns: table => new
                {
                    Step_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkFlow_ID = table.Column<int>(type: "int", nullable: false),
                    Level_ID = table.Column<int>(type: "int", nullable: false),
                    Step_Order = table.Column<int>(type: "int", nullable: false),
                    Role_ID = table.Column<int>(type: "int", nullable: false),
                    Designation_ID = table.Column<int>(type: "int", nullable: false),
                    IsFinal_Step = table.Column<bool>(type: "bit", nullable: false),
                    Created_On = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowSteps", x => x.Step_ID);
                    table.ForeignKey(
                        name: "FK_WorkFlowSteps_Master_Level_Level_ID",
                        column: x => x.Level_ID,
                        principalTable: "Master_Level",
                        principalColumn: "Level_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkFlowSteps_Master_WorkFlow_WorkFlow_ID",
                        column: x => x.WorkFlow_ID,
                        principalTable: "Master_WorkFlow",
                        principalColumn: "WorkFlow_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Master_WorkFlow_StateID",
                table: "Master_WorkFlow",
                column: "StateID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowSteps_Level_ID",
                table: "WorkFlowSteps",
                column: "Level_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowSteps_WorkFlow_ID",
                table: "WorkFlowSteps",
                column: "WorkFlow_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkFlowSteps");

            migrationBuilder.DropTable(
                name: "Master_Level");

            migrationBuilder.DropTable(
                name: "Master_WorkFlow");
        }
    }
}
