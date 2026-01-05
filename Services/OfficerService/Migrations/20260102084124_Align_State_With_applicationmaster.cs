using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficerService.Migrations
{
    /// <inheritdoc />
    public partial class Align_State_With_applicationmaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_Master_State_State_ID",
                table: "Application_Master");

            migrationBuilder.AlterColumn<int>(
                name: "ST_CODE",
                schema: "dbo",
                table: "State",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_State_ST_CODE",
                schema: "dbo",
                table: "State",
                column: "ST_CODE");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Master_State_State_ID",
                table: "Application_Master",
                column: "State_ID",
                principalSchema: "dbo",
                principalTable: "State",
                principalColumn: "ST_CODE",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_Master_State_State_ID",
                table: "Application_Master");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_State_ST_CODE",
                schema: "dbo",
                table: "State");

            migrationBuilder.AlterColumn<int>(
                name: "ST_CODE",
                schema: "dbo",
                table: "State",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Master_State_State_ID",
                table: "Application_Master",
                column: "State_ID",
                principalSchema: "dbo",
                principalTable: "State",
                principalColumn: "State_Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
