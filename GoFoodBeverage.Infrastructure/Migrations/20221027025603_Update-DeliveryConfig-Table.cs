using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateDeliveryConfigTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "DeliveryConfig");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "DeliveryConfig",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DeliveryConfig",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "DeliveryConfig",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "DeliveryConfig");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DeliveryConfig");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "DeliveryConfig");

            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "DeliveryConfig",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
