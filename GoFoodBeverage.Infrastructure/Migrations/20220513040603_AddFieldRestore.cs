using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddFieldRestore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestoreData",
                table: "Material");

            migrationBuilder.AddColumn<string>(
                name: "RestoreData",
                table: "PurchaseOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestoreData",
                table: "PurchaseOrder");

            migrationBuilder.AddColumn<string>(
                name: "RestoreData",
                table: "Material",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
