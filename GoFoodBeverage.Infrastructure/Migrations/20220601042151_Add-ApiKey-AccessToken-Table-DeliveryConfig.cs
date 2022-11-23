using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddApiKeyAccessTokenTableDeliveryConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "DeliveryConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "DeliveryConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
              table: "DeliveryMethod",
              columns: new[] { "Id", "EnumId", "Name", "IsEnabled" },
              values: new object[,] {
                    {Guid.NewGuid(), (int)EnumDeliveryMethod.AhaMove, "AhaMove", false}
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "DeliveryConfig");

            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "DeliveryConfig");
        }
    }
}
