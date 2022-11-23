using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddExpiryMembershipDateToLoyaltyPointTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryMembershipDate",
                table: "LoyaltyPointConfig",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExpiryMembershipDate",
                table: "LoyaltyPointConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryMembershipDate",
                table: "LoyaltyPointConfig");

            migrationBuilder.DropColumn(
                name: "IsExpiryMembershipDate",
                table: "LoyaltyPointConfig");
        }
    }
}
