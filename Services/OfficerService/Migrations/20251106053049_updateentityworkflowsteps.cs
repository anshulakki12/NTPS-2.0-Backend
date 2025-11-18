using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updateentityworkflowsteps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowSteps_Designation_ID",
                table: "WorkFlowSteps",
                column: "Designation_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowSteps_Role_ID",
                table: "WorkFlowSteps",
                column: "Role_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlowSteps_Master_Designation_Designation_ID",
                table: "WorkFlowSteps",
                column: "Designation_ID",
                principalTable: "Master_Designation",
                principalColumn: "Designation_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlowSteps_Master_Roles_Role_ID",
                table: "WorkFlowSteps",
                column: "Role_ID",
                principalTable: "Master_Roles",
                principalColumn: "Role_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlowSteps_Master_Designation_Designation_ID",
                table: "WorkFlowSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlowSteps_Master_Roles_Role_ID",
                table: "WorkFlowSteps");

            migrationBuilder.DropIndex(
                name: "IX_WorkFlowSteps_Designation_ID",
                table: "WorkFlowSteps");

            migrationBuilder.DropIndex(
                name: "IX_WorkFlowSteps_Role_ID",
                table: "WorkFlowSteps");
        }
    }
}
