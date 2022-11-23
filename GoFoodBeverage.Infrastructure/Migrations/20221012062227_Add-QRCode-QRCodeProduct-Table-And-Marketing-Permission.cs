using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddQRCodeQRCodeProductTableAndMarketingPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "OrderHistory",
                type: "nvarchar(255)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QRCode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StoreBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false, comment: "EnumOrderType get 2 values: Instore(0) and Online(3)"),
                    AreaTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TargetId = table.Column<int>(type: "int", nullable: false, comment: "EnumTargetQRCode have 2 values: ShopMenu(0) and AddProductToCart(1)"),
                    IsPercentDiscount = table.Column<bool>(type: "bit", nullable: false, comment: "If the value is true, have percent discount"),
                    PercentNumber = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MaximumDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsStopped = table.Column<bool>(type: "bit", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QRCodeProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QRCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductQuantity = table.Column<int>(type: "int", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRCodeProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QRCodeProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QRCodeProduct_QRCode_QRCodeId",
                        column: x => x.QRCodeId,
                        principalTable: "QRCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QRCodeProduct_ProductId",
                table: "QRCodeProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_QRCodeProduct_QRCodeId",
                table: "QRCodeProduct",
                column: "QRCodeId");

            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/Scripts/s16-marketing-permission.sql");
            var script = File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QRCodeProduct");

            migrationBuilder.DropTable(
                name: "QRCode");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "OrderHistory");
        }
    }
}
