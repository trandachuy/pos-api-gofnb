using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class UpdatetableBillConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShowPassword",
                table: "BillConfiguration");

            migrationBuilder.RenameColumn(
                name: "IsShowWifi",
                table: "BillConfiguration",
                newName: "IsShowWifiAndPassword");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsShowWifiAndPassword",
                table: "BillConfiguration",
                newName: "IsShowWifi");

            migrationBuilder.AddColumn<bool>(
                name: "IsShowPassword",
                table: "BillConfiguration",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
