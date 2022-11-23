using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateOrderTableAndCreateOrderDeliveryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderDeliveryId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderDelivery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderLat = table.Column<double>(type: "float", nullable: true),
                    SenderLng = table.Column<double>(type: "float", nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverLat = table.Column<double>(type: "float", nullable: true),
                    ReceiverLng = table.Column<double>(type: "float", nullable: true),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDelivery", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderDeliveryId",
                table: "Order",
                column: "OrderDeliveryId",
                unique: true,
                filter: "[OrderDeliveryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderDelivery_OrderDeliveryId",
                table: "Order",
                column: "OrderDeliveryId",
                principalTable: "OrderDelivery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_OrderDelivery_OrderDeliveryId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "OrderDelivery");

            migrationBuilder.DropIndex(
                name: "IX_Order_OrderDeliveryId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderDeliveryId",
                table: "Order");
        }
    }
}
