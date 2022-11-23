using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoFoodBeverage.Infrastructure.Migrations
{
    public partial class InitChannelData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "Channel",
              columns: new[] { "Id", "Name" },
              values: new object[,] {
                    {(int)EnumChannel.InStore, EnumChannel.InStore.GetDescription()},
                    { (int)EnumChannel.Grab, EnumChannel.Grab.GetDescription()}
              }
          );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
