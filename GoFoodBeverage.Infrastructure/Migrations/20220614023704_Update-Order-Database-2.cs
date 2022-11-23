using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateOrderDatabase2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "Order",
                newName: "TotalDiscountAmount");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaximumDiscount",
                table: "CustomerMembershipLevel",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalDiscountAmount",
                table: "Order",
                newName: "DiscountAmount");

            migrationBuilder.AlterColumn<int>(
                name: "MaximumDiscount",
                table: "CustomerMembershipLevel",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
