using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationService.Migrations
{
    /// <inheritdoc />
    public partial class addtablemasterroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MasterRolesRoleId",
                table: "Applicant_Registration",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Master_Roles",
                columns: table => new
                {
                    Role_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Updated_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_By = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Deactivated_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reactivated_On = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Roles", x => x.Role_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_Registration_MasterRolesRoleId",
                table: "Applicant_Registration",
                column: "MasterRolesRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicant_Registration_Master_Roles_MasterRolesRoleId",
                table: "Applicant_Registration",
                column: "MasterRolesRoleId",
                principalTable: "Master_Roles",
                principalColumn: "Role_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicant_Registration_Master_Roles_MasterRolesRoleId",
                table: "Applicant_Registration");

            migrationBuilder.DropTable(
                name: "Master_Roles");

            migrationBuilder.DropIndex(
                name: "IX_Applicant_Registration_MasterRolesRoleId",
                table: "Applicant_Registration");

            migrationBuilder.DropColumn(
                name: "MasterRolesRoleId",
                table: "Applicant_Registration");
        }
    }
}
