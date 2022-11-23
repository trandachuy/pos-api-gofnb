using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddColumnPlatformIdToOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlatformId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_PlatformId",
                table: "Order",
                column: "PlatformId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Platform_PlatformId",
                table: "Order",
                column: "PlatformId",
                principalTable: "Platform",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Platform_PlatformId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_PlatformId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PlatformId",
                table: "Order");
        }
    }
}
