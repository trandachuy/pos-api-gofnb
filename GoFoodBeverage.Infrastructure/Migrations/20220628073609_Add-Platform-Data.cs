using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class AddPlatformData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "Platform",
              columns: new[] { "Id", "Name", "StatusId" },
              values: new object[,] {
                     { (int) EnumPlatform.POS, EnumPlatform.POS.ToString(), (int)EnumStatus.Active},
                    { (int) EnumPlatform.GoFnBApp, EnumPlatform.GoFnBApp.ToString(), (int)EnumStatus.Active},
              }
          );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
