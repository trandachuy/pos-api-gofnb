using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateOrderDatabase3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductPriceValue",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "OrderItem",
                newName: "PriceAfterDiscount");

            migrationBuilder.AddColumn<string>(
                name: "CashierName",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shift_StaffId",
                table: "Shift",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shift_Staff_StaffId",
                table: "Shift",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shift_Staff_StaffId",
                table: "Shift");

            migrationBuilder.DropIndex(
                name: "IX_Shift_StaffId",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "CashierName",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "PriceAfterDiscount",
                table: "OrderItem",
                newName: "Price");

            migrationBuilder.AddColumn<decimal>(
                name: "ProductPriceValue",
                table: "OrderItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
