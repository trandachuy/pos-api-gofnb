using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateOrderItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPromotionDiscountPercentage",
                table: "OrderItem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "OrderItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductPriceName",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProductPriceValue",
                table: "OrderItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PromotionDiscountValue",
                table: "OrderItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "PromotionId",
                table: "OrderItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AreaTableId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPromotionDiscountPercentage",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PromotionDiscountValue",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "PromotionId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TableId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderFee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPercentage = table.Column<bool>(type: "bit", nullable: false),
                    FeeValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderFee_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OptionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionLevelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemTopping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToppingName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToppingValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemTopping", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_AreaTableId",
                table: "Order",
                column: "AreaTableId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFee_OrderId",
                table: "OrderFee",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AreaTable_AreaTableId",
                table: "Order",
                column: "AreaTableId",
                principalTable: "AreaTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AreaTable_AreaTableId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "OrderFee");

            migrationBuilder.DropTable(
                name: "OrderItemOption");

            migrationBuilder.DropTable(
                name: "OrderItemTopping");

            migrationBuilder.DropIndex(
                name: "IX_Order_AreaTableId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsPromotionDiscountPercentage",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ProductPriceName",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ProductPriceValue",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PromotionDiscountValue",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "AreaTableId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsPromotionDiscountPercentage",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PromotionDiscountValue",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TableId",
                table: "Order");
        }
    }
}
