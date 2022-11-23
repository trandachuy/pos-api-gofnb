using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddIsAllMatchCustomerSegment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                table: "CustomerSegment");

            migrationBuilder.DropColumn(
                name: "EnumCustomerSegmentConditionId",
                table: "CustomerSegment");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "CustomerSegment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnumCustomerSegmentConditionId",
                table: "CustomerSegment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
