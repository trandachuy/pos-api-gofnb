using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateOrderPackageColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RemainAmount",
                table: "OrderPackage",
                newName: "BranchPurchaseRemainAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "OrderPackage",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "OrderPackage");

            migrationBuilder.RenameColumn(
                name: "BranchPurchaseRemainAmount",
                table: "OrderPackage",
                newName: "RemainAmount");
        }
    }
}
