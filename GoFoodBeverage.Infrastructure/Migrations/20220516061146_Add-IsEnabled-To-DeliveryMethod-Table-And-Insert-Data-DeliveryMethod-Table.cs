using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddIsEnabledToDeliveryMethodTableAndInsertDataDeliveryMethodTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "DeliveryMethod",
                type: "bit",
                nullable: true);

            migrationBuilder.InsertData(
              table: "DeliveryMethod",
              columns: new[] { "Id", "EnumId", "Name", "IsEnabled"},
              values: new object[,] {
                    {Guid.NewGuid(), (int)EnumDeliveryMethod.SelfDelivery, "Self Delivery", false}
              }
          );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "DeliveryMethod");
        }
    }
}
