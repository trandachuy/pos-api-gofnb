using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddColumnOrderToTableGroupPermissionFunctionPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "PermissionGroup",
                type: "int",
                nullable: true,
                comment: "The priority of permission group");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "FunctionGroup",
                type: "int",
                nullable: true,
                comment: "The priority of function group");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "PermissionGroup");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "FunctionGroup");
        }
    }
}
