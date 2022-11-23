using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class updateactivateaccountstoreintostoretable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "Account");

            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "Store",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "Store");

            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
