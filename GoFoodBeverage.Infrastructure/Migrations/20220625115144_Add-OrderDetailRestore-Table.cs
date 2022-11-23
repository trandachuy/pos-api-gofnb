using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddOrderDetailRestoreTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderComboProductPriceItemId",
                table: "OrderItemTopping",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderComboProductPriceItemId",
                table: "OrderItemOption",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCombo",
                table: "OrderItem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "OrderComboItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ComboId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ComboPricingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ComboName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderComboItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderComboItem_OrderItem_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderRestore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AreaTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    OrderPaymentStatusId = table.Column<int>(type: "int", nullable: false),
                    OrderTypeId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    StringCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPromotionDiscountPercentage = table.Column<bool>(type: "bit", nullable: false),
                    PromotionDiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PromotionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerMemberShipLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerAccumulatedPoint = table.Column<int>(type: "int", nullable: true),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaTable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderFees = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRestore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderComboProductPriceItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderComboItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductPriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderComboProductPriceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderComboProductPriceItem_OrderComboItem_OrderComboItemId",
                        column: x => x.OrderComboItemId,
                        principalTable: "OrderComboItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemRestore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductPriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderRestoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductPriceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceAfterDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPromotionDiscountPercentage = table.Column<bool>(type: "bit", nullable: false),
                    PromotionDiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PromotionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCombo = table.Column<bool>(type: "bit", nullable: false),
                    ProductPrice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Promotion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderItemOptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderItemToppings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemRestore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemRestore_OrderRestore_OrderRestoreId",
                        column: x => x.OrderRestoreId,
                        principalTable: "OrderRestore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemTopping_OrderComboProductPriceItemId",
                table: "OrderItemTopping",
                column: "OrderComboProductPriceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemOption_OrderComboProductPriceItemId",
                table: "OrderItemOption",
                column: "OrderComboProductPriceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderComboItem_OrderItemId",
                table: "OrderComboItem",
                column: "OrderItemId",
                unique: true,
                filter: "[OrderItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderComboProductPriceItem_OrderComboItemId",
                table: "OrderComboProductPriceItem",
                column: "OrderComboItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemRestore_OrderRestoreId",
                table: "OrderItemRestore",
                column: "OrderRestoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemOption_OrderComboProductPriceItem_OrderComboProductPriceItemId",
                table: "OrderItemOption",
                column: "OrderComboProductPriceItemId",
                principalTable: "OrderComboProductPriceItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemTopping_OrderComboProductPriceItem_OrderComboProductPriceItemId",
                table: "OrderItemTopping",
                column: "OrderComboProductPriceItemId",
                principalTable: "OrderComboProductPriceItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemOption_OrderComboProductPriceItem_OrderComboProductPriceItemId",
                table: "OrderItemOption");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemTopping_OrderComboProductPriceItem_OrderComboProductPriceItemId",
                table: "OrderItemTopping");

            migrationBuilder.DropTable(
                name: "OrderComboProductPriceItem");

            migrationBuilder.DropTable(
                name: "OrderItemRestore");

            migrationBuilder.DropTable(
                name: "OrderComboItem");

            migrationBuilder.DropTable(
                name: "OrderRestore");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemTopping_OrderComboProductPriceItemId",
                table: "OrderItemTopping");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemOption_OrderComboProductPriceItemId",
                table: "OrderItemOption");

            migrationBuilder.DropColumn(
                name: "OrderComboProductPriceItemId",
                table: "OrderItemTopping");

            migrationBuilder.DropColumn(
                name: "OrderComboProductPriceItemId",
                table: "OrderItemOption");

            migrationBuilder.DropColumn(
                name: "IsCombo",
                table: "OrderItem");
        }
    }
}
