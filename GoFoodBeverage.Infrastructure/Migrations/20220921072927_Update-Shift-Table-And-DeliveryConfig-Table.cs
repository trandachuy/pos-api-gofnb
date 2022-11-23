using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdateShiftTableAndDeliveryConfigTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "DeliveryMethod");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Shift",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                comment: "The database generates a value when a row is inserted");

            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "DeliveryConfig",
                type: "bit",
                nullable: true,
                comment: "If the value is true, the delivery method is not supported");

            var sqlFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Migrations/Scripts/s14-fix-funtion-permission-promotion-data.sql");
            var script = System.IO.File.ReadAllText(sqlFile);
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "DeliveryConfig");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "DeliveryMethod",
                type: "bit",
                nullable: true);
        }
    }
}
