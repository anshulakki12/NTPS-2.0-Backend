using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOfficerDetailsAndAddRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Officer_Login_id",
                table: "Officer_Details",
                newName: "Login_id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Officer_Registration_Login_Id",
                table: "Officer_Registration",
                column: "Login_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Officer_Registration_Login_Id",
                table: "Officer_Registration",
                column: "Login_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Officer_Registration_role_id",
                table: "Officer_Registration",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_Officer_Details_Login_id",
                table: "Officer_Details",
                column: "Login_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Officer_Details_Officer_Registration_Login_id",
                table: "Officer_Details",
                column: "Login_id",
                principalTable: "Officer_Registration",
                principalColumn: "Login_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Officer_Registration_Master_Roles_role_id",
                table: "Officer_Registration",
                column: "role_id",
                principalTable: "Master_Roles",
                principalColumn: "Role_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Officer_Details_Officer_Registration_Login_id",
                table: "Officer_Details");

            migrationBuilder.DropForeignKey(
                name: "FK_Officer_Registration_Master_Roles_role_id",
                table: "Officer_Registration");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Officer_Registration_Login_Id",
                table: "Officer_Registration");

            migrationBuilder.DropIndex(
                name: "IX_Officer_Registration_Login_Id",
                table: "Officer_Registration");

            migrationBuilder.DropIndex(
                name: "IX_Officer_Registration_role_id",
                table: "Officer_Registration");

            migrationBuilder.DropIndex(
                name: "IX_Officer_Details_Login_id",
                table: "Officer_Details");

            migrationBuilder.RenameColumn(
                name: "Login_id",
                table: "Officer_Details",
                newName: "Officer_Login_id");
        }
    }
}
