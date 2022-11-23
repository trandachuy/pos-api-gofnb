using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddOrderSessionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderSessionId",
                table: "OrderItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantityCompleted",
                table: "OrderItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "OrderItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OrderSession",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    LastSavedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSavedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSession_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderSessionId",
                table: "OrderItem",
                column: "OrderSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSession_OrderId",
                table: "OrderSession",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_OrderSession_OrderSessionId",
                table: "OrderItem",
                column: "OrderSessionId",
                principalTable: "OrderSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_OrderSession_OrderSessionId",
                table: "OrderItem");

            migrationBuilder.DropTable(
                name: "OrderSession");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_OrderSessionId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "OrderSessionId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "QuantityCompleted",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "OrderItem");
        }
    }
}
