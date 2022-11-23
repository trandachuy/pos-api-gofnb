using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class RemoveActionEnumFromOrderPackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountPackageAction",
                table: "OrderPackage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountPackageAction",
                table: "OrderPackage",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
