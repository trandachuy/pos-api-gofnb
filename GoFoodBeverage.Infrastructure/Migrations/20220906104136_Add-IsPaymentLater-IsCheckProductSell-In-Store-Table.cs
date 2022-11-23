using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddIsPaymentLaterIsCheckProductSellInStoreTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCheckProductSell",
                table: "Store",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Allowing to sell products when out of materials");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaymentLater",
                table: "Store",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Customer will pay after finishing the meal (after create the order)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCheckProductSell",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "IsPaymentLater",
                table: "Store");
        }
    }
}
