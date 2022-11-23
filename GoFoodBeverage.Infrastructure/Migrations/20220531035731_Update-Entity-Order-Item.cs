using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateEntityOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderItemTopping_OrderItemId",
                table: "OrderItemTopping",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemOption_OrderItemId",
                table: "OrderItemOption",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemOption_OrderItem_OrderItemId",
                table: "OrderItemOption",
                column: "OrderItemId",
                principalTable: "OrderItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemTopping_OrderItem_OrderItemId",
                table: "OrderItemTopping",
                column: "OrderItemId",
                principalTable: "OrderItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemOption_OrderItem_OrderItemId",
                table: "OrderItemOption");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemTopping_OrderItem_OrderItemId",
                table: "OrderItemTopping");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemTopping_OrderItemId",
                table: "OrderItemTopping");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemOption_OrderItemId",
                table: "OrderItemOption");
        }
    }
}
