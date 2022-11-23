using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddSomeFieldsToOrderPackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerName",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShopName",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShopPhoneNumber",
                table: "OrderPackage",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "OrderPackage");

            migrationBuilder.DropColumn(
                name: "SellerName",
                table: "OrderPackage");

            migrationBuilder.DropColumn(
                name: "ShopName",
                table: "OrderPackage");

            migrationBuilder.DropColumn(
                name: "ShopPhoneNumber",
                table: "OrderPackage");
        }
    }
}
