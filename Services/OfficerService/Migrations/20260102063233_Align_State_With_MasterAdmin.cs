using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class Align_State_With_MasterAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop FK if it exists
            migrationBuilder.DropForeignKey(
                name: "FK_Application_Master_State_State_ID",
                table: "Application_Master");

            // Re-create FK with correct PK
            migrationBuilder.AddForeignKey(
                name: "FK_Application_Master_State_State_ID",
                table: "Application_Master",
                column: "State_ID",
                principalSchema: "dbo",
                principalTable: "State",
                principalColumn: "State_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
               name: "FK_Application_Master_State_State_ID",
               table: "Application_Master");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Master_State_State_ID",
                table: "Application_Master",
                column: "State_ID",
                principalSchema: "dbo",
                principalTable: "State",
                principalColumn: "State_Id");
        }
    }
}
