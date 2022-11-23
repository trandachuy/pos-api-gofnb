using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateOrderDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GrossTotalAmount",
                table: "Order",
                newName: "OriginalPrice");

            migrationBuilder.AlterColumn<decimal>(
                name: "PercentNumber",
                table: "Promotion",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<Guid>(
                name: "ToppingId",
                table: "OrderItemTopping",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OptionId",
                table: "OrderItemOption",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OptionLevelId",
                table: "OrderItemOption",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_PromotionId",
                table: "OrderItem",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Promotion_PromotionId",
                table: "OrderItem",
                column: "PromotionId",
                principalTable: "Promotion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Promotion_PromotionId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_PromotionId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ToppingId",
                table: "OrderItemTopping");

            migrationBuilder.DropColumn(
                name: "OptionId",
                table: "OrderItemOption");

            migrationBuilder.DropColumn(
                name: "OptionLevelId",
                table: "OrderItemOption");

            migrationBuilder.RenameColumn(
                name: "OriginalPrice",
                table: "Order",
                newName: "GrossTotalAmount");

            migrationBuilder.AlterColumn<float>(
                name: "PercentNumber",
                table: "Promotion",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
