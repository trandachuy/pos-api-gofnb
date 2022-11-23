using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddCodeColumnToAccountTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerCode",
                table: "OrderPackage",
                newName: "AccountCode");

            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "Account",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "AccountCode",
                table: "OrderPackage",
                newName: "CustomerCode");
        }
    }
}
