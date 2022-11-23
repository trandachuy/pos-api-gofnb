using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateFunctionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Function_Package_PackageId",
                table: "Function");

            migrationBuilder.DropIndex(
                name: "IX_Function_PackageId",
                table: "Function");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Function");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "Function",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Function_PackageId",
                table: "Function",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Function_Package_PackageId",
                table: "Function",
                column: "PackageId",
                principalTable: "Package",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
