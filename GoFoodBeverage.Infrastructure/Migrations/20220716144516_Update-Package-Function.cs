using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdatePackageFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
               name: "IX_PackageFunction_FunctionGroupId",
               table: "PackageFunction");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageFunction_FunctionGroup_FunctionGroupId",
                table: "PackageFunction");

            migrationBuilder.DropColumn(
                name: "FunctionGroupId",
                table: "PackageFunction");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FunctionGroupId",
                table: "PackageFunction",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageFunction_FunctionGroup_FunctionGroupId",
                table: "PackageFunction",
                column: "FunctionGroupId",
                principalTable: "FunctionGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
