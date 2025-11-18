using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class updateapplicationmastertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Forest_Produce_ID",
                table: "Application_Master",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Application_Master_District_ID",
                table: "Application_Master",
                column: "District_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Master_Forest_Produce_ID",
                table: "Application_Master",
                column: "Forest_Produce_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Master_State_ID",
                table: "Application_Master",
                column: "State_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Application_Master_SubDistrict_ID",
                table: "Application_Master",
                column: "SubDistrict_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Master_DISTRICT_District_ID",
                table: "Application_Master",
                column: "District_ID",
                principalTable: "DISTRICT",
                principalColumn: "DIST_CODE");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Master_Forest_Produce_Forest_Produce_ID",
                table: "Application_Master",
                column: "Forest_Produce_ID",
                principalTable: "Forest_Produce",
                principalColumn: "Forest_Produce_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Master_State_State_ID",
                table: "Application_Master",
                column: "State_ID",
                principalSchema: "dbo",
                principalTable: "State",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Master_Sub_District_SubDistrict_ID",
                table: "Application_Master",
                column: "SubDistrict_ID",
                principalTable: "Sub_District",
                principalColumn: "Sub_Dist_Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_Master_DISTRICT_District_ID",
                table: "Application_Master");

            migrationBuilder.DropForeignKey(
                name: "FK_Application_Master_Forest_Produce_Forest_Produce_ID",
                table: "Application_Master");

            migrationBuilder.DropForeignKey(
                name: "FK_Application_Master_State_State_ID",
                table: "Application_Master");

            migrationBuilder.DropForeignKey(
                name: "FK_Application_Master_Sub_District_SubDistrict_ID",
                table: "Application_Master");

            migrationBuilder.DropIndex(
                name: "IX_Application_Master_District_ID",
                table: "Application_Master");

            migrationBuilder.DropIndex(
                name: "IX_Application_Master_Forest_Produce_ID",
                table: "Application_Master");

            migrationBuilder.DropIndex(
                name: "IX_Application_Master_State_ID",
                table: "Application_Master");

            migrationBuilder.DropIndex(
                name: "IX_Application_Master_SubDistrict_ID",
                table: "Application_Master");

            migrationBuilder.DropColumn(
                name: "Forest_Produce_ID",
                table: "Application_Master");
        }
    }
}
