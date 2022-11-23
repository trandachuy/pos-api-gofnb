using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class RemoveTaxCodeAndFullName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreBankAccount_City_CityId",
                table: "StoreBankAccount");

            migrationBuilder.RenameColumn(
                name: "TaxCode",
                table: "StoreBankAccount",
                newName: "SwiftCode");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "StoreBankAccount",
                newName: "RoutingNumber");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "StoreBankAccount",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreBankAccount_City_CityId",
                table: "StoreBankAccount",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreBankAccount_City_CityId",
                table: "StoreBankAccount");

            migrationBuilder.RenameColumn(
                name: "SwiftCode",
                table: "StoreBankAccount",
                newName: "TaxCode");

            migrationBuilder.RenameColumn(
                name: "RoutingNumber",
                table: "StoreBankAccount",
                newName: "FullName");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "StoreBankAccount",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreBankAccount_City_CityId",
                table: "StoreBankAccount",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
