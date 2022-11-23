using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateOrderPackageDataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderBranchPurchasePackageId",
                table: "OrderPackage",
                newName: "ActivateStorePackageId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "OrderPackage",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPackage_ActivateStorePackageId",
                table: "OrderPackage",
                column: "ActivateStorePackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderPackage_OrderPackage_ActivateStorePackageId",
                table: "OrderPackage",
                column: "ActivateStorePackageId",
                principalTable: "OrderPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderPackage_OrderPackage_ActivateStorePackageId",
                table: "OrderPackage");

            migrationBuilder.DropIndex(
                name: "IX_OrderPackage_ActivateStorePackageId",
                table: "OrderPackage");

            migrationBuilder.RenameColumn(
                name: "ActivateStorePackageId",
                table: "OrderPackage",
                newName: "OrderBranchPurchasePackageId");

            migrationBuilder.AlterColumn<int>(
                name: "TotalAmount",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
