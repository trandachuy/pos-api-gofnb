using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddProductPriceRelationshipAndStatusIdForOrderComboProductPriceItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "OrderComboProductPriceItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderComboProductPriceItem_ProductPriceId",
                table: "OrderComboProductPriceItem",
                column: "ProductPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderComboProductPriceItem_ProductPrice_ProductPriceId",
                table: "OrderComboProductPriceItem",
                column: "ProductPriceId",
                principalTable: "ProductPrice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderComboProductPriceItem_ProductPrice_ProductPriceId",
                table: "OrderComboProductPriceItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderComboProductPriceItem_ProductPriceId",
                table: "OrderComboProductPriceItem");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "OrderComboProductPriceItem");
        }
    }
}
